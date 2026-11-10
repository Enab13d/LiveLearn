using System.Text.Json;
using LiveLearn.Ordering.Application.Configuration;
using LiveLearn.Ordering.Application.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace LiveLearn.Ordering.Infrastructure.Services;

// Hash cart:{studentId}, field = CourseId, value = JSON { addedAt, priceAtAdd }; EXPIRE refreshed on every change (O3).
// ct is accepted for interface symmetry but not forwarded: StackExchange.Redis takes no tokens, its command timeouts
// are set on the multiplexer. Don't wrap calls in WaitAsync(ct) — that only abandons the await, the command still runs.
internal sealed class RedisCartStore(
    IConnectionMultiplexer redis,
    IOptions<CartOptions> options,
    ILogger<RedisCartStore> logger) : ICartStore
{
    // One round trip, so two parallel adds can't both pass the duplicate / size checks.
    // Returns 0 → AlreadyInCart, 1 → Added, 2 → CartFull (mapped in AddAsync).
    private static readonly LuaScript AddScript = LuaScript.Prepare("""
        if redis.call('HEXISTS', @key, @field) == 1 then return 0 end
        if redis.call('HLEN', @key) >= tonumber(@maxItems) then return 2 end
        redis.call('HSET', @key, @field, @value)
        redis.call('PEXPIRE', @key, @ttlMs)
        return 1
        """);

    private sealed record CartEntry(DateTimeOffset AddedAt, decimal PriceAtAdd);

    public async Task<IReadOnlyList<CartItem>> GetAsync(Guid studentId, CancellationToken ct)
    {
        var fields = await Db.HashGetAllAsync(Key(studentId));

        var items = new List<CartItem>(fields.Length);
        foreach (var field in fields)
        {
            if (TryRead(field) is { } item) items.Add(item);
            else logger.LogWarning("Skipped unreadable cart field {Field} of student {StudentId}", field.Name.ToString(), studentId);
        }

        return items.OrderBy(i => i.AddedAt).ToList();
    }

    // A single bad field (e.g. after a CartEntry schema change) is skipped instead of making the whole cart unreadable.
    private static CartItem? TryRead(HashEntry field)
    {
        if (!Guid.TryParse(field.Name.ToString(), out var courseId)) return null;

        try
        {
            var entry = JsonSerializer.Deserialize<CartEntry>(field.Value.ToString(), JsonSerializerOptions.Web);
            return entry is null ? null : new CartItem(courseId, entry.AddedAt, entry.PriceAtAdd);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    public async Task<CartAddResult> AddAsync(Guid studentId, CartItem item, CancellationToken ct)
    {
        var value = JsonSerializer.Serialize(new CartEntry(item.AddedAt, item.PriceAtAdd), JsonSerializerOptions.Web);

        var result = (int)await Db.ScriptEvaluateAsync(AddScript, new
        {
            key = (RedisKey)Key(studentId),
            field = item.CourseId.ToString(),
            value,
            maxItems = options.Value.MaxItems,
            ttlMs = (long)options.Value.Ttl.TotalMilliseconds
        });

        return result switch
        {
            0 => CartAddResult.AlreadyInCart,
            2 => CartAddResult.CartFull,
            _ => CartAddResult.Added
        };
    }

    public async Task RemoveAsync(Guid studentId, IReadOnlyCollection<Guid> courseIds, CancellationToken ct)
    {
        if (courseIds.Count == 0) return;

        var key = Key(studentId);
        var transaction = Db.CreateTransaction();
        _ = transaction.HashDeleteAsync(key, courseIds.Select(id => (RedisValue)id.ToString()).ToArray());
        // No-op when the delete emptied the hash — Redis drops the key itself.
        _ = transaction.KeyExpireAsync(key, options.Value.Ttl);
        await transaction.ExecuteAsync();
    }

    public async Task ClearAsync(Guid studentId, CancellationToken ct) => await Db.KeyDeleteAsync(Key(studentId));

    private IDatabase Db => redis.GetDatabase();

    private static string Key(Guid studentId) => $"cart:{studentId}";
}

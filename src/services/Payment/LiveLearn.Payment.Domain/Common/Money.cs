using LiveLearn.BuildingBlocks;
using LiveLearn.Payment.Domain.Errors;

namespace LiveLearn.Payment.Domain.Common;


// An amount in a currency (D8). Kept at full decimal precision; Round() brings it to the currency's
// precision at calculation points. Minor units (cents) exist only in the Stripe adapter.
public sealed record Money
{
    // Supported currencies and their decimal places — USD only for now (D1).
    private static readonly Dictionary<string, int> _decimalPlaces = new() { ["USD"] = 2 };

    public decimal Amount { get; }
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Result<Money> Create(decimal amount, string currency)
    {
        if (!_decimalPlaces.ContainsKey(currency))
            return Result<Money>.Failure(MoneyErrors.UnsupportedCurrency(currency));

        return new Money(amount, currency);
    }

    public Money Round() =>
        new(Math.Round(Amount, _decimalPlaces[Currency], MidpointRounding.ToEven), Currency);

    // A currency mismatch is a programming error, not a business one: every Money is created through
    // Create, and the aggregates only combine amounts of a single currency.
    public static Money operator +(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return new Money(left.Amount + right.Amount, left.Currency);
    }

    public static Money operator -(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return new Money(left.Amount - right.Amount, left.Currency);
    }

    private static void EnsureSameCurrency(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException($"Cannot combine {left.Currency} and {right.Currency} amounts.");
    }
}

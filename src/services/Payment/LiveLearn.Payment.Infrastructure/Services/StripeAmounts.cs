using LiveLearn.Payment.Domain.Common;

namespace LiveLearn.Payment.Infrastructure.Services;


// Minor units exist only in the Stripe adapter (D8). USD only (D1): two decimal places.
internal static class StripeAmounts
{
    public static long ToMinorUnits(Money money) => (long)(money.Round().Amount * 100);

    public static Money FromMinorUnits(long amount, string currency)
    {
        var money = Money.Create(amount / 100m, currency.ToUpperInvariant());
        return money.IsSuccess
            ? money.Value
            : throw new InvalidOperationException($"Stripe returned an unsupported currency '{currency}'.");
    }
}

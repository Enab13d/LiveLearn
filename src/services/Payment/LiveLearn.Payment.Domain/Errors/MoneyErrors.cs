using LiveLearn.BuildingBlocks;

namespace LiveLearn.Payment.Domain.Errors;


public static class MoneyErrors
{
    private static readonly string _prefix = nameof(MoneyErrors);

    public static Error UnsupportedCurrency(string currency) =>
        Error.Unprocessable($"{_prefix}.UnsupportedCurrency", $"Currency '{currency}' is not supported");
}

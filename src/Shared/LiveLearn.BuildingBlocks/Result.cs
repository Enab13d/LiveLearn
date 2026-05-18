namespace LiveLearn.BuildingBlocks;

public class Result
{
    private readonly bool _isSuccess;

    public bool IsSuccess => _isSuccess;
    public string[]? Errors { get; }

    protected Result(bool isSuccess, string[]? errors = null)
    {
        _isSuccess = isSuccess;
        Errors = errors;
    }

    public static Result Success() => new(true);
    public static Result Failure(params string[] errors) => new(false, errors);
}

public sealed class Result<T> : Result
{
    private readonly T? _value;

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access Value on a failed result.");

    private Result(bool isSuccess, T? value = default, string[]? errors = null)
        : base(isSuccess, errors)
    {
        _value = value;
    }

    public static Result<T> Success(T value) => new(true, value);
    public static new Result<T> Failure(params string[] errors) => new(false, errors: errors);

    public static implicit operator Result<T>(T value) => Success(value);
}

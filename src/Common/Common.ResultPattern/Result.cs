namespace Common.ResultPattern;
 
public interface IResult
{
    bool IsSuccess { get; }
}

public record Result : IResult
{
    public bool IsSuccess { get; protected set; }
    public bool IsFailure => !IsSuccess;
    public Error? Error { get; protected set; }

    internal Result() => IsSuccess = true;
    internal Result(Error? error) => (Error, IsSuccess) = (error, false);

    public static Result Success() => new();
    public static Result<T> Success<T>(T value) => new(value);

    public static Result Failure(Error? error) => new(error);
    public static Result Failure(string code, string message) => new(new Error(code, message));
    public static Result Failure(string message) => new(new Error("", message));


    public static Result<T> Failure<T>(Error? error) => new(error);
    public static Result<T> Failure<T>(string code, string message) => Failure<T>(new Error(code, message));

    public Result WithError(Error error)
    {
        IsSuccess = false;
        Error = error;
        return this;
    }

    public bool HasError<TError>()
    {
        return Error is not null && typeof(TError) == Error.GetType();
    }
}

public sealed record Result<T> : Result
{
    private readonly T? _value;

    public T Value => IsSuccess ? _value! : throw new InvalidOperationException("Result is failure");

    internal Result(T value) => _value = value;

    internal Result(Error? error) : base(error)
    {
    }
}

public static class ResultExtensions
{
    public static Result<T> ToFailedResult<T>(this Error error)
        => Result.Failure<T>(error);
}
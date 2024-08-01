namespace PAMAi.Application;

/// <summary>
/// Represents the result of an operation in the application.
/// </summary>
/// <remarks>
/// This should be used to avoid the performance overhead of throwing exceptions.
/// Using <see cref="Result{T}"/>, the caller method knows exactly what to expect 
/// from the called method, and it is easier to know if the operation failed or not.
/// <para>
/// See <seealso href="https://github.com/dotnet/aspnetcore/issues/46280#issuecomment-1527898867">GitHub discussion.</seealso>.
/// </para>
/// </remarks>
/// <typeparam name="T">Data type of the resulting data.</typeparam>
public sealed record Result<T>: Result
{
    private Result(bool isSuccess, Error error, T? data) : base(isSuccess, error)
    {
        Data = data;
    }

    /// <summary>
    /// Data returned by the operation.
    /// </summary>
    public T? Data { get; }

    /// <summary>
    /// Creates a <see cref="Result{T}"/> that indicates a successful operation.
    /// </summary>
    /// <returns>A successful <see cref="Result{T}"/>.</returns>
    public static new Result<T> Success() => new(true, Error.None, default);

    /// <summary>
    /// Creates a <see cref="Result{T}"/> that indicates a successful operation.
    /// </summary>
    /// <param name="data">Resulting data from the operation.</param>
    /// <returns>A successful <see cref="Result{T}"/>.</returns>
    public static Result<T> Success(T data) => new(true, Error.None, data);

    /// <summary>
    /// Creates a <see cref="Result{T}"/> that indicates a failed operation.
    /// </summary>
    /// <param name="error">Error from the operation.</param>
    /// <returns>An unsuccessful <see cref="Result{T}"/>.</returns>
    public static new Result<T> Failure(Error error) => new(false, error, default);

    /// <summary>
    /// Creates a <see cref="Result{T}"/> that indicates a failed operation.
    /// </summary>
    /// <param name="error">Error from the operation.</param>
    /// <param name="data">Resulting data from the operation.</param>
    /// <returns>An unsuccessful <see cref="Result{T}"/>.</returns>
    public static Result<T> Failure(Error error, T data) => new(false, error, data);
}

/// <summary>
/// Represents the result of an operation in the application.
/// </summary>
/// <remarks>
/// This should be used to avoid the performance overhead of throwing exceptions.
/// Using <see cref="Result"/>, the caller method knows exactly what to expect 
/// from the called method, and it is easier to know if the operation failed or not.
/// <para>
/// See <seealso href="https://github.com/dotnet/aspnetcore/issues/46280#issuecomment-1527898867">GitHub discussion.</seealso>.
/// </para>
/// </remarks>
public record Result
{
    protected Result(bool isSuccess, Error error)
    {
        if ((isSuccess && error != Error.None) ||
            (!isSuccess && error == Error.None))
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    /// Indicates if the operation was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Indicates if the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Error that caused the result to fail.
    /// </summary>
    public Error Error { get; }

    /// <summary>
    /// Creates a <see cref="Result"/> that indicates a successful operation.
    /// </summary>
    /// <returns>A successful <see cref="Result"/>.</returns>
    public static Result Success() => new(true, Error.None);

    /// <summary>
    /// Creates a <see cref="Result"/> that indicates a failed operation.
    /// </summary>
    /// <param name="error">Error from the operation.</param>
    /// <returns>An unsuccessful <see cref="Result"/>.</returns>
    public static Result Failure(Error error) => new(false, error);

    /// <summary>
    /// Creates a <see cref="Result"/> from a <see cref="Result{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="result"></param>
    /// <returns>
    /// A <see cref="Result"/> indicating success if <paramref name="result"/> was successful.
    /// </returns>
    public static Result From<T>(Result<T> result)
    {
        return result.IsSuccess ? Success() : Failure(result.Error);
    }
}
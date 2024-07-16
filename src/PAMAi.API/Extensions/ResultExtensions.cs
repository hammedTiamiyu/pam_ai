namespace PAMAi.API.Extensions;

public static class ResultExtensions
{
    /// <summary>
    /// Matches the given result with an appropriate <see cref="IActionResult"/>
    /// to be returned from the controller.
    /// </summary>
    /// <param name="result">The <see cref="Result"/>.</param>
    /// <param name="onSuccess">Function to run if the result indicates success.</param>
    /// <param name="onFailure">Function to run if the result indicates failure.</param>
    /// <returns>An <see cref="IActionResult"/>.</returns>
    public static IActionResult Match(this Result result, Func<IActionResult> onSuccess, Func<Error, IActionResult> onFailure)
    {
        return result.IsSuccess ? onSuccess() : onFailure(result.Error);
    }

    /// <summary>
    /// Matches the given result with an appropriate <see cref="IActionResult"/>
    /// to be returned from the controller.
    /// </summary>
    /// <param name="result">The <see cref="Result{T}"/>.</param>
    /// <param name="onSuccess">Function to run if the result indicates success.</param>
    /// <param name="onFailure">Function to run if the result indicates failure.</param>
    /// <returns>An <see cref="IActionResult"/>.</returns>
    public static IActionResult Match<T>(this Result<T> result, Func<T, IActionResult> onSuccess, Func<T, Error, IActionResult> onFailure)
    {
        return result.IsSuccess ? onSuccess(result.Data) : onFailure(result.Data, result.Error);
    }
}

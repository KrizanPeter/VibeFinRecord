using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace FinClaude.Api.Common.Errors;

public static class ErrorOrExtensions
{
    public static IActionResult ToActionResult<T>(this ErrorOr<T> result, Func<T, IActionResult> onValue)
    {
        if (!result.IsError)
            return onValue(result.Value);

        var error = result.FirstError;
        return error.Type switch
        {
            ErrorType.Validation => new UnprocessableEntityObjectResult(ToProblemDetails(error, 422)),
            ErrorType.NotFound   => new NotFoundObjectResult(ToProblemDetails(error, 404)),
            ErrorType.Conflict   => new ConflictObjectResult(ToProblemDetails(error, 409)),
            _                    => new ObjectResult(ToProblemDetails(error, 500)) { StatusCode = 500 },
        };
    }

    private static ProblemDetails ToProblemDetails(Error error, int status) => new()
    {
        Status = status,
        Title = error.Code,
        Detail = error.Description,
    };
}

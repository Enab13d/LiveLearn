using LiveLearn.BuildingBlocks;
using Microsoft.AspNetCore.Mvc;

namespace LiveLearn.Assessment.API.Extensions;

internal static class ResultExtensions
{
    public static IActionResult ToProblemResult(this ControllerBase controller, Error[] errors)
    {
        if (errors.All(e => e.Type == ErrorType.Validation))
        {
            foreach (var error in errors)
                controller.ModelState.AddModelError(error.Code, error.Message);
            return controller.ValidationProblem();
        }

        var first = errors[0];
        var statusCode = first.Type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };

        return controller.Problem(statusCode: statusCode, detail: first.Message, title: first.Code);
    }

    public static IActionResult MisconfiguredTokenProblem(this ControllerBase controller) =>
        controller.Problem("Token sub claim is not a valid identifier. Identity provider misconfigured.");
}

using Common.ResultPattern;
using MediaService.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace MediaService.Api.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult(
        this Result result,
        ControllerBase controller)
    {
        return result.IsSuccess ? controller.NoContent() : Problem(result, controller);
    }

    public static ActionResult<T> ToActionResult<T>(
        this Result<T> result,
        ControllerBase controller)
    {
        return result.IsSuccess ? controller.Ok(result.Value) : Problem(result, controller);
    }
 
    private static ObjectResult Problem(Result result, ControllerBase controller)
    {
        return controller.Problem(
            title: result.Error?.Code,
            detail: result.Error?.Message,
            statusCode: MapStatusCode(result.Error),
            extensions: new Dictionary<string, object?>
            {
                ["errorCode"] = result.Error?.Code
            });
    }


    private static int MapStatusCode(Error? error)
    {
        return error switch
        {
            MediaNotFoundError => StatusCodes.Status404NotFound,
            DuplicateLinkError => StatusCodes.Status409Conflict,
            // ContentTypeIsNotAllowedError => StatusCodes.Status403Forbidden,
            // MediaIsNotAvailableError => StatusCodes.Status400BadRequest,
            // AttachmentMustHaveOnlyOneLinkError => StatusCodes.Status400BadRequest,
            // MediaWithLinksCantBeRemovedError => StatusCodes.Status400BadRequest,
            FileSizeExceededError => StatusCodes.Status400BadRequest,

            _ => StatusCodes.Status400BadRequest
        };
    }
}
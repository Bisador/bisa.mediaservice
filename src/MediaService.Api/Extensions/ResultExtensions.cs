using Common.ResultPattern;
using MediaService.Application.Abstractions;
using MediaService.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace MediaService.Api.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult(
        this Result result,
        ControllerBase controller)
    {
        if (result.IsSuccess)
            return controller.NoContent();

        return controller.Problem(
            title: result.Error?.Code,
            detail: result.Error?.Message,
            statusCode: MapStatusCode(result.Error));
    }

    public static ActionResult<T> ToActionResult<T>(
        this Result<T> result,
        ControllerBase controller)
    {
        if (result.IsSuccess)
            return controller.Ok(result.Value);

        return controller.Problem(
            title: result.Error?.Code,
            detail: result.Error?.Message,
            statusCode: MapStatusCode(result.Error));
    }

    private static int MapStatusCode(Error? error)
    {
        return error switch
        {
            MediaNotFoundError => StatusCodes.Status404NotFound,

            DuplicateLinkError => StatusCodes.Status409Conflict,

            FileSizeExceededError => StatusCodes.Status400BadRequest,

            _ => StatusCodes.Status500InternalServerError
        };
    }
}
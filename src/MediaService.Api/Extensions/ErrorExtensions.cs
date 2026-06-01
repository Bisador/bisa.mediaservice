using Common.ResultPattern; 
using Microsoft.AspNetCore.Mvc;

namespace MediaService.Api.Extensions;

public static class ErrorExtensions
{
    public static ProblemDetails ToProblemDetails(
        this Error error)
    {
        return error switch
        {  
            _ =>
                new ProblemDetails
                {
                    Title = error.Message,
                    Status = StatusCodes.Status400BadRequest
                }
        };
    }
}
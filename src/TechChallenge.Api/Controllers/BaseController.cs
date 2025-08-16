using System.Net;
using Microsoft.AspNetCore.Mvc;
using OperationResult;

namespace TechChallenge.Api.Controllers;

[ApiController]
public class BaseController : ControllerBase
{
    protected IActionResult Send(object? value)
        => value switch
        {
            null => Problem(statusCode: (int)HttpStatusCode.NotFound),
            _ => Ok(value)
        };

    protected async Task<IActionResult> Send(Task<Result<Guid>> task)
        => await task switch
        {
            (true, var result) => Created(string.Empty, result),
            (false, _, var exception) => TreatError(exception),
            _ => Problem(statusCode: (int)HttpStatusCode.InternalServerError)
        };
    
    protected async Task<IActionResult> Send<TResponse>(Task<Result<TResponse>> task)
        => await task switch
        {
            (true, var result) => Ok(result),
            (false, _, var exception) => TreatError(exception),
            _ => Problem(statusCode: (int)HttpStatusCode.InternalServerError)
        };
    
    protected async Task<IActionResult> Send(Task<Result> task)
        => await task switch
        {
            (true, _) => Ok(),
            (false, var exception) => TreatError(exception)
        };
    
    [NonAction]
    private ObjectResult TreatError(Exception? error) => error switch
    {
        UnauthorizedAccessException => Unauthorized(error.Message),
        not null => Problem(detail: error.Message, statusCode: (int)HttpStatusCode.BadRequest),
        _ => Problem(statusCode: (int)HttpStatusCode.InternalServerError)
    };
}
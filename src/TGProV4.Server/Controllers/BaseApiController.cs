using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace TGProV4.Server.Controllers;

/// <summary>
/// Abstract BaseApi Controller Class
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    private IMediator? _mediatorInstance;

    protected IMediator Mediator => _mediatorInstance ??= HttpContext.RequestServices.GetService<IMediator>()!;

    protected ActionResult HandleResult<T>(T result, HttpStatusCode statusCode)
    {
        if (result is not null && !result.Equals(default))
        {
            return Ok(new Response<T> { Message = statusCode.ToString(), Data = result });
        }

        return BadRequest(new Response<string>
        {
            Succeeded = false, Data = default, Message = HttpStatusCode.BadRequest.ToString()
        });
    }
}

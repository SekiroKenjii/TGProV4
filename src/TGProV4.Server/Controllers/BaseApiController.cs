namespace TGProV4.Server.Controllers;

/// <summary>
/// Abstract BaseApi Controller Class
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    private IMediator? _mediatorInstance;

    protected IMediator Mediator {
        get => (_mediatorInstance ??= HttpContext.RequestServices.GetService<IMediator>()) ??
               throw new ArgumentNullException(nameof(_mediatorInstance));
    }

    protected ActionResult HandleResult<T>(T result, HttpStatusCode statusCode)
    {
        if (result is not null && !result.Equals(default))
        {
            return Ok(new Response<T> {
                Message = statusCode.ToString(),
                Data = result,
                Succeeded = true
            });
        }

        if (statusCode is HttpStatusCode.OK && result is null && typeof(T) != typeof(bool))
        {
            return NotFound(new Response<T> {
                Message = HttpStatusCode.NotFound.ToString(),
                Data = result,
                Succeeded = false
            });
        }

        return BadRequest(new Response<T> {
            Succeeded = false,
            Data = result,
            Message = HttpStatusCode.BadRequest.ToString()
        });
    }
}

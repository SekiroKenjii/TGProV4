namespace TGProV4.Server.Middlewares;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlerMiddleware> _logger;

    public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, exception.Message);
            await HandleException(context, exception);
        }
    }

    private static async Task HandleException(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var response = new Response<string>
        {
            Succeeded = false,
            Data = default
        };
        
        switch (exception)
        {
            case NotFoundException  notFound:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Message = notFound.Message;
                break;
            case Application.Exceptions.ValidationException validation:
                context.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                response.Message = validation.Message;
                response.Errors = validation.GetErrors();
                break;
            case SecurityTokenException tokenError:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response.Message = tokenError.Message;
                break;
            case BadRequestException bad:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = bad.Message;
                break;
            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Message = "Internal server error";
                response.Errors.Add(new BaseError
                {
                    Code = exception.Source,
                    RelatedProperties = null,
                    Message = exception.Message
                });
                break;
        }

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        var json = System.Text.Json.JsonSerializer.Serialize(response, options);

        await context.Response.WriteAsync(json);
    }
}
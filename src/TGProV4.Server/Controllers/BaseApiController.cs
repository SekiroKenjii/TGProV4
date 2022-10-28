using Microsoft.AspNetCore.Mvc;

namespace TGProV4.Server.Controllers;

/// <summary>
/// Abstract BaseApi Controller Class
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class BaseApiController : ControllerBase
{
}
using Microsoft.AspNetCore.Mvc;
using TGProV4.Application.Features.Productions.Brand.Queries;

namespace TGProV4.Server.Controllers.v1.Catalog;

public class BrandsController : BaseApiController
{
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var brand = await Mediator.Send(new GetBrandById { Id = id });
        return HandleResult(brand, HttpStatusCode.OK);
    }
}

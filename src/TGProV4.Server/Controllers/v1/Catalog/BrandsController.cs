using Microsoft.AspNetCore.Mvc;
using TGProV4.Application.Features.Productions.Brand.Queries;

namespace TGProV4.Server.Controllers.v1.Catalog;

public class BrandsController : BaseApiController
{
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetBrand(int id)
    {
        var response = await Mediator.Send(new GetBrandById.Query { Id = id });

        return HandleResult(response, HttpStatusCode.OK);
    }

    [HttpGet]
    public async Task<IActionResult> GetBrands()
    {
        var response = await Mediator.Send(new GetBrands.Query());

        return HandleResult(response, HttpStatusCode.OK);
    }
}

namespace TGProV4.Server.Controllers.v1.Catalog;

public class BrandsController : BaseApiController
{
    [HttpGet("{id:int}")]
    [Authorize(Policy = ApplicationPermissions.Brands.Read)]
    public async Task<IActionResult> GetBrand(int id)
    {
        var response = await Mediator.Send(new GetBrandById.Query { Id = id });

        return HandleResult(response, HttpStatusCode.OK);
    }

    [HttpGet]
    [Authorize(Policy = ApplicationPermissions.Brands.Read)]
    public async Task<IActionResult> GetBrands()
    {
        var response = await Mediator.Send(new GetBrands.Query());

        return HandleResult(response, HttpStatusCode.OK);
    }

    [HttpPost]
    [Authorize(Policy = ApplicationPermissions.Brands.Create)]
    public async Task<IActionResult> CreateBrand([FromForm] UpsertBrandRequest request)
    {
        var response = await Mediator.Send(new CreateBrand.Command { Brand = request });

        return HandleResult(response, HttpStatusCode.Created);
    }
}

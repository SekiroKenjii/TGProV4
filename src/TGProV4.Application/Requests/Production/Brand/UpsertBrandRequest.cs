namespace TGProV4.Application.Requests.Production.Brand;

public class UpsertBrandRequest : ImageUploadRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
}

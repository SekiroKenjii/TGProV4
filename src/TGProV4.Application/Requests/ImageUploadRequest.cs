namespace TGProV4.Application.Requests;

public class ImageUploadRequest
{
    public void SetEntity(string entity)
    {
        Entity = entity;
    }

    public string? Entity { get; private set; }
    public IFormFile? ImageFile { get; set; }
}

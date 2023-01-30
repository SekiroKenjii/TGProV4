namespace TGProV4.Application.Requests;

public class ImageUploadRequest
{
    public string? Entity { get; set; }
    public IFormFile? ImageFile { get; set; }
}

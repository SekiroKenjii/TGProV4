namespace TGProV4.Application.Interfaces.Services.Cloud;

public interface IImageService<in T> where T : class
{
    Task<ImageUploadResponse?> Upload(T request);
    Task<string> Remove(string publicId);
}

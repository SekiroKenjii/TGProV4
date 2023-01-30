using CloudinaryConfiguration=TGProV4.Application.Configurations.CloudinaryConfiguration;

namespace TGProV4.Infrastructure.Services.Cloud;

public class ImageService<T> : IImageService<T> where T : ImageUploadRequest
{
    private readonly Cloudinary _cloudinary;

    public ImageService(IOptions<CloudinaryConfiguration> config) => _cloudinary = new Cloudinary(new Account
    (
        config.Value.CloudName,
        config.Value.ApiKey,
        config.Value.ApiSecret
    ));

    public async Task<ImageUploadResponse?> Upload(T request)
    {
        var uploadParams = new ImageUploadParams();

        if (request.ImageFile is null)
        {
            if (string.IsNullOrEmpty(request.Entity))
            {
                return null;
            }

            return request switch {
                UpdateUserProfileRequest => GetDefaultUserImage(request.Entity),
                _ => GetDefaultProductionImage(request.Entity)
            };
        }

        await using var fileStream = request.ImageFile.OpenReadStream();

        uploadParams.Folder = $"TGProV3/{request.Entity}/";
        uploadParams.File = new FileDescription(request.ImageFile.FileName, fileStream);

        uploadParams.Transformation = request switch {
            UpsertProductRequest => new Transformation().Height(800).Width(800).Crop("fill"),
            _ => new Transformation().Height(500).Crop("fill")
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error is not null)
        {
            throw new Exception(uploadResult.Error.Message);
        }

        return new ImageUploadResponse {
            ImageUrl = uploadResult.SecureUrl.ToString(),
            PublicId = uploadResult.PublicId
        };
    }

    public async Task<string> Remove(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(deleteParams);

        return result.Result;
    }

    private static ImageUploadResponse GetDefaultUserImage(string? gender = null)
    {
        return gender switch {
            nameof(Gender.Female) => new ImageUploadResponse {
                ImageUrl = ApplicationConstants.DefaultImages.FemaleAvatar,
                PublicId = ApplicationConstants.DefaultImages.FemaleAvatarId
            },
            _ => new ImageUploadResponse {
                ImageUrl = ApplicationConstants.DefaultImages.MaleAvatar,
                PublicId = ApplicationConstants.DefaultImages.MaleAvatarId
            }
        };
    }

    private static ImageUploadResponse GetDefaultProductionImage(string entity)
    {
        return entity switch {
            ApplicationConstants.Entities.Brand => new ImageUploadResponse {
                ImageUrl = ApplicationConstants.DefaultImages.BrandImage,
                PublicId = ApplicationConstants.DefaultImages.BrandImageId
            },
            _ => new ImageUploadResponse {
                ImageUrl = ApplicationConstants.DefaultImages.ProductImage,
                PublicId = ApplicationConstants.DefaultImages.ProductImageId
            }
        };
    }
}

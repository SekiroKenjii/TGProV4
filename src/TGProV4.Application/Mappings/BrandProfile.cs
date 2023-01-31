namespace TGProV4.Application.Mappings;

// ReSharper disable once UnusedType.Global
public class BrandProfile : Profile
{
    public BrandProfile()
    {
        CreateMap<BrandResponse, Brand>().ReverseMap();
        CreateMap<Brand, UpsertBrandRequest>().ReverseMap();
    }
}

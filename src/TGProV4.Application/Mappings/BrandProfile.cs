namespace TGProV4.Application.Mappings;

public class BrandProfile : Profile
{
    public BrandProfile()
    {
        CreateMap<QueryBrandResponse, Brand>().ReverseMap();
    }
}

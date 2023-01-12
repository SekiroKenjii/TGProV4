namespace TGProV4.Infrastructure.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
        => CreateMap<RegisterRequest, AppUser>()
          .ForMember(d => d.IsActive,
               o => o.MapFrom(s => s.ActivateUser))
          .ForMember(d => d.EmailConfirmed,
               o => o.MapFrom(s => s.AutoConfirmEmail));
}

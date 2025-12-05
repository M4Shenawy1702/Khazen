using Khazen.Application.DOTs.Auth;

namespace Khazen.Application.MappingProfile.UserModule
{
    public class ApplicationUserProfile : Profile
    {
        public ApplicationUserProfile()
        {
            CreateMap<ApplicationUser, ApplicationUserDto>();
            CreateMap<IdentityRole, RoleDto>();
            CreateMap<IdentityRole, RoleDetailsDto>();

        }
    }
}


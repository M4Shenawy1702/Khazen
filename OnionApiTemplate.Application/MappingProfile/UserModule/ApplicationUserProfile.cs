using Khazen.Application.DOTs.Auth;

namespace Khazen.Application.MappingProfile.UserModule
{
    public class ApplicationUserProfile : Profile
    {
        public ApplicationUserProfile()
        {
            CreateMap<ApplicationUser, ApplicationUserDto>();
            CreateMap<ApplicationRole, RoleDto>();
            CreateMap<ApplicationRole, RoleDetailsDto>();

        }
    }
}


namespace Khazen.Application.DOTs.Auth
{
    public class RoleDetailsDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public IList<string> Claims { get; set; } = [];
        public IList<ApplicationUserDto> Users { get; set; } = [];
    }
}

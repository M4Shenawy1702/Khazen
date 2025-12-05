namespace Khazen.Application.MappingProfile
{
    internal class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            CreateMap<Employee, EmployeeDetailsDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User!.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User!.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User!.PhoneNumber))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.User!.Address))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User!.FullName))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.User!.Gender.ToString()))
                .ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => src.JobTitle.ToString()))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.User!.DateOfBirth))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.User!.IsActive))
                .ForMember(dest => dest.LastLoginAt, opt => opt.MapFrom(src => src.User!.LastLoginAt))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department!.Name))
                .ForMember(dest => dest.Salary, opt => opt.MapFrom(src => src.BaseSalary))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.ModifiedAt, opt => opt.MapFrom(src => src.ModifiedAt))
                .ForMember(dest => dest.TotalBonuses, opt => opt.MapFrom(src => src.Bonuses.Sum(x => x.Amount)))
                .ForMember(dest => dest.TotalAdvances, opt => opt.MapFrom(src => src.Advances.Sum(x => x.Amount)))
                .ForMember(dest => dest.TotalDeductions, opt => opt.MapFrom(src => src.Deductions.Sum(x => x.Amount)))
                .ForMember(dest => dest.NetSalary, opt => opt.MapFrom(src => (src.BaseSalary + src.Bonuses.Sum(x => x.Amount)) - (src.Advances.Sum(x => x.Amount) + src.Deductions.Sum(x => x.Amount))));

            CreateMap<Employee, EmployeeDto>()
               .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => src.JobTitle.ToString()))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department!.Name))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.User!.IsActive));

        }
    }
}

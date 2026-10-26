using Application.Common.Mappings;
using AutoMapper;

namespace Application.Features.Users.Employees.Dtos;

public class EmployeeDto : IMapFrom<Domain.Users.Employee>
{
    public Guid Id { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Domain.Users.Employee, EmployeeDto>();
    }
}
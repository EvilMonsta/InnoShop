using AutoMapper;
using InnoShop.Users.Contracts.Responses;
using InnoShop.Users.Domain.Users;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace InnoShop.Users.Application.Mapping;

public class UsersMappingProfile : Profile
{
    public UsersMappingProfile()
    {
        CreateMap<User, UserResponse>()
            .ForCtorParam("Id", opt => opt.MapFrom(s => s.Id))
            .ForCtorParam("Name", opt => opt.MapFrom(s => s.Name))
            .ForCtorParam("Email", opt => opt.MapFrom(s => s.Email))
            .ForCtorParam("Role", opt => opt.MapFrom(s => s.Role.ToString()))
            .ForCtorParam("IsActive", opt => opt.MapFrom(s => s.IsActive))
            .ForCtorParam("EmailConfirmed", opt => opt.MapFrom(s => s.EmailConfirmed))
            .ForCtorParam("CreatedAt", opt => opt.MapFrom(s => s.CreatedAt.ToString("O")));
    }
}

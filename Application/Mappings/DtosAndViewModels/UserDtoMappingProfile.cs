using AutoMapper;
using InvestmentApp.Core.Application.Dtos.User;
using InvestmentApp.Core.Application.ViewModels.Asset;
using InvestmentApp.Core.Application.ViewModels.User;

namespace InvestmentApp.Core.Application.Mappings.DtosAndViewModels
{
    public class UserDtoMappingProfile : Profile
    {
        public UserDtoMappingProfile() {
           
            CreateMap<UserDto, UserViewModel>()             
                .ReverseMap();
        
            CreateMap<UserDto, DeleteUserViewModel>()      
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ReverseMap()          
                .ForMember(dest => dest.Email, opt => opt.Ignore())
                .ForMember(dest => dest.UserName, opt => opt.Ignore())
                .ForMember(dest => dest.Phone, opt => opt.Ignore())
                .ForMember(dest => dest.ProfileImage, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore());
     
            CreateMap<UserDto, UpdateUserViewModel>()          
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.ConfirmPassword, opt => opt.Ignore())
                .ForMember(dest => dest.ProfileImageFile, opt => opt.Ignore())
                .ReverseMap()             
                .ForMember(dest => dest.ProfileImage, opt => opt.Ignore());

            CreateMap<SaveUserDto, UpdateUserViewModel>()
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.ConfirmPassword, opt => opt.Ignore())
                .ForMember(dest => dest.ProfileImageFile, opt => opt.Ignore())
                .ReverseMap()      
                .ForMember(dest => dest.ProfileImage, opt => opt.Ignore());

            CreateMap<SaveUserDto, CreateUserViewModel>()                          
                .ForMember(dest => dest.ProfileImageFile, opt => opt.Ignore())
                .ReverseMap()          
                .ForMember(dest => dest.ProfileImage, opt => opt.Ignore());
     
            CreateMap<SaveUserDto, RegisterUserViewModel>()                
                .ForMember(dest => dest.ProfileImageFile, opt => opt.Ignore())
                .ReverseMap()                               
                .ForMember(dest => dest.ProfileImage, opt => opt.Ignore());

            CreateMap<LoginDto, LoginViewModel>()
                .ReverseMap();
        }
    }
}

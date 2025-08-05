using InvestmentApp.Core.Application.Dtos.User;

namespace InvestmentApp.Core.Application.Interfaces
{
    public interface IBaseAccountService
    {
        Task<UserResponseDto> ConfirmAccountAsync(string userId, string token);
        Task<UserResponseDto> DeleteAsync(string id);
        Task<EditResponseDto> EditUser(SaveUserDto saveDto, string? origin, bool? isCreated = false, bool? isApi = false);
        Task<UserResponseDto> ForgotPasswordAsync(ForgotPasswordRequestDto request, bool? isApi = false);
        Task<List<UserDto>> GetAllUser(bool? isActive = true);
        Task<UserDto?> GetUserByEmail(string email);
        Task<UserDto?> GetUserById(string Id);
        Task<UserDto?> GetUserByUserName(string userName);
        Task<RegisterResponseDto> RegisterUser(SaveUserDto saveDto, string? origin, bool? isApi = false);
        Task<UserResponseDto> ResetPasswordAsync(ResetPasswordRequestDto request);
    }
}
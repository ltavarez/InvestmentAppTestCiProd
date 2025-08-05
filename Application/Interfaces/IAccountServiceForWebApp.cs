using InvestmentApp.Core.Application.Dtos.User;

namespace InvestmentApp.Core.Application.Interfaces
{
    public interface IAccountServiceForWebApp : IBaseAccountService
    {
        Task<LoginResponseDto> AuthenticateAsync(LoginDto loginDto); 
        Task SignOutAsync();
    }
}
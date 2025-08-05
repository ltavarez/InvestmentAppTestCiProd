using InvestmentApp.Core.Application.Dtos.User;

namespace InvestmentApp.Core.Application.Interfaces
{
    public interface IAccountServiceForWebApi : IBaseAccountService
    {
        Task<LoginResponseForApiDto> AuthenticateAsync(LoginDto loginDto);
    }
}
using InvestmentApp.Core.Application.Dtos.Email;

namespace InvestmentApp.Core.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(EmailRequestDto emailRequestDto);
    }
}
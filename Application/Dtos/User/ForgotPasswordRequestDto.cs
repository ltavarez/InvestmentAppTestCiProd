namespace InvestmentApp.Core.Application.Dtos.User
{
    public class ForgotPasswordRequestDto
    {
        public required string UserName { get; set; }
        public string? Origin { get; set; }
    }
}

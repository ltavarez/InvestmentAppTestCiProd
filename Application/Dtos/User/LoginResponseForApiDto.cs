namespace InvestmentApp.Core.Application.Dtos.User
{
    public class LoginResponseForApiDto
    {
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public bool HasError { get; set; }
        public required List<string> Errors { get; set; }
        public string? AccessToken { get; set; }
    }
}

namespace InvestmentApp.Core.Application.Dtos.User
{
    public class UserResponseDto
    {
        public string? Message {  get; set; }
        public bool HasError { get; set; }
        public required List<string> Errors { get; set; }
    }
}

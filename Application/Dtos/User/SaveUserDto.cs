namespace InvestmentApp.Core.Application.Dtos.User
{
    public class SaveUserDto
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public string? Phone { get; set; }
        public string? ProfileImage { get; set; }
        public required string Role { get; set; }
    }
}

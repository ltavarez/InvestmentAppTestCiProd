namespace InvestmentApp.Core.Application.ViewModels.User
{
    public class UserViewModel
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string UserName { get; set; }
        public string? Phone { get; set; }
        public string? ProfileImage { get; set; }
        public required string Role { get; set; }
    }
}

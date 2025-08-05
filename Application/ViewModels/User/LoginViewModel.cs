using System.ComponentModel.DataAnnotations;

namespace InvestmentApp.Core.Application.ViewModels.User
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "You must enter the username of user")]
        [DataType(DataType.Text)]
        public required string UserName { get; set; }

        [Required(ErrorMessage = "You must enter the password of user")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}

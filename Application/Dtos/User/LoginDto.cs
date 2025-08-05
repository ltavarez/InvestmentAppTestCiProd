using Swashbuckle.AspNetCore.Annotations;

namespace InvestmentApp.Core.Application.Dtos.User
{
    /// <summary>
    /// Parameters required to authenticate a user
    /// </summary>
    public class LoginDto
    {
        /// <example>admin</example>
        [SwaggerParameter(Description = "The username or email used to log in")]
        public required string UserName { get; set; }

        /// <example>123Pa$$word!</example>
        [SwaggerParameter(Description = "The user's login password")]
        public required string Password { get; set; }
    }
}

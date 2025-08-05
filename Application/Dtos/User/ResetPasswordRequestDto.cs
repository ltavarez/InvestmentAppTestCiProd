using Swashbuckle.AspNetCore.Annotations;

namespace InvestmentApp.Core.Application.Dtos.User
{
    /// <summary>
    /// Parameters required to reset a user's password using a valid token
    /// </summary>
    public class ResetPasswordRequestDto
    {
        /// <example>8f9d2a3b-5c4a-42a7-b6f7-0e9e8bdb178a</example>
        [SwaggerParameter(Description = "The unique identifier of the user")]
        public required string UserId { get; set; }

        /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...</example>
        [SwaggerParameter(Description = "The token received for password reset")]
        public required string Token { get; set; }

        /// <example>NewP@ssword2025!</example>
        [SwaggerParameter(Description = "The new password to be set for the user")]
        public required string Password { get; set; }
    }
}

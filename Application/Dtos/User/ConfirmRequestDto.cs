using Swashbuckle.AspNetCore.Annotations;

namespace InvestmentApp.Core.Application.Dtos.User
{
    /// <summary>
    /// Parameters required to confirm a user account
    /// </summary>
    public class ConfirmRequestDto
    {
        /// <example>8f9d2a3b-5c4a-42a7-b6f7-0e9e8bdb178a</example>
        [SwaggerParameter(Description = "The unique identifier of the user to confirm")]
        public required string UserId { get; set; }

        /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...</example>
        [SwaggerParameter(Description = "The confirmation token sent to the user's email")]
        public required string Token { get; set; }
    }
}

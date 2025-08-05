using Swashbuckle.AspNetCore.Annotations;

namespace InvestmentApp.Core.Application.Dtos.User
{
    /// <summary>
    /// Parameters required to request a password reset token
    /// </summary>
    public class ForgotPasswordApiRequestDto
    {
        /// <example>juanp</example>
        [SwaggerParameter(Description = "The username of the account requesting password reset")]
        public required string UserName { get; set; }
    }
}

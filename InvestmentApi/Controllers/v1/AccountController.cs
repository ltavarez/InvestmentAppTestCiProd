using Asp.Versioning;
using InvestmentApi.Handlers;
using InvestmentApp.Core.Application.Dtos.User;
using InvestmentApp.Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InvestmentApi.Controllers.v1
{
    [ApiVersion("1.0")]
    [SwaggerTag("Endpoints for user authentication, registration, and account recovery")]
    public class AccountController(IAccountServiceForWebApi accountServiceForWebApi) : BaseApiController
    {
        private readonly IAccountServiceForWebApi _accountServiceForWebApi = accountServiceForWebApi;

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Authenticate user",
            Description = "Validates user credentials and returns an authentication token with user information"
        )]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            return Ok(await _accountServiceForWebApi.AuthenticateAsync(dto));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Register a new user",
            Description = "Creates a new user account with profile image upload support"
        )]
        public async Task<IActionResult> Register([FromForm] CreateUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var save = new SaveUserDto
            {
                Id = "",
                Email = dto.Email,
                LastName = dto.LastName,
                Name = dto.Name,
                Password = dto.Password,
                Phone = dto.Phone,
                Role = dto.Role.ToString(),
                UserName = dto.UserName,
                ProfileImage = ""
            };

            var result = await _accountServiceForWebApi.RegisterUser(save, null, true);

            if (result == null || result.HasError)
                return BadRequest(result?.Errors);

            save.Id = result.Id;
            save.ProfileImage = FileHandler.Upload(dto.ProfileImage, result.Id, "Users");

            var resultEdit = await _accountServiceForWebApi.EditUser(save, null, true, true);

            if (resultEdit == null || resultEdit.HasError)
                return BadRequest(resultEdit?.Errors);

            return StatusCode(StatusCodes.Status201Created);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("confirm-account")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Confirm user account",
            Description = "Validates and confirms a user's account using a token"
        )]
        public async Task<IActionResult> Confirm([FromBody] ConfirmRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = await _accountServiceForWebApi.ConfirmAccountAsync(dto.UserId, dto.Token);

            if (result == null || result.HasError)
                return BadRequest(result?.Message);

            return Ok(result.Message);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("get-reset-token")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Generate password reset token",
            Description = "Generates a secure token for password recovery and sends it via email"
        )]
        public async Task<IActionResult> GetResetToken([FromBody] ForgotPasswordApiRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = await _accountServiceForWebApi.ForgotPasswordAsync(
                new ForgotPasswordRequestDto { UserName = dto.UserName }, true);

            if (result == null || result.HasError)
                return BadRequest(result?.Errors);

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("change-password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Reset user password",
            Description = "Resets the user's password using the provided reset token"
        )]
        public async Task<IActionResult> ChangePassword([FromBody] ResetPasswordRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = await _accountServiceForWebApi.ResetPasswordAsync(dto);

            if (result == null || result.HasError)
                return BadRequest(result?.Errors);

            return NoContent();
        }
    }
}

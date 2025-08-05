using Asp.Versioning;
using InvestmentApi.Handlers;
using InvestmentApp.Core.Application.Dtos.User;
using InvestmentApp.Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InvestmentApi.Controllers.v1
{
    [Authorize(Roles = "Admin")]
    [ApiVersion("1.0")]
    [SwaggerTag("Administrative operations to manage user accounts")]
    public class UserController(IAccountServiceForWebApi accountServiceForWebApi) : BaseApiController
    {
        private readonly IAccountServiceForWebApi _accountServiceForWebApi = accountServiceForWebApi;

        [HttpGet]
        [SwaggerOperation(Summary = "Get all users", Description = "Retrieves all registered users")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<UserDto>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            var users = await _accountServiceForWebApi.GetAllUser();
            if (users == null || users.Count == 0)
                return NoContent();

            return Ok(users);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get user by ID", Description = "Retrieves a user by their unique identifier")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(string id)
        {
            var user = await _accountServiceForWebApi.GetUserById(id);
            if (user == null) return NoContent();

            return Ok(user);
        }

        [HttpGet("username/{userName}")]
        [SwaggerOperation(Summary = "Get user by username", Description = "Retrieves a user using their username")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByUserName(string userName)
        {
            var user = await _accountServiceForWebApi.GetUserByUserName(userName);
            if (user == null) return NoContent();

            return Ok(user);
        }

        [HttpGet("email/{email}")]
        [SwaggerOperation(Summary = "Get user by email", Description = "Retrieves a user using their email address")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByUserEmail(string email)
        {
            var user = await _accountServiceForWebApi.GetUserByEmail(email);
            if (user == null) return NoContent();

            return Ok(user);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create a new user", Description = "Registers a new user with profile image")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromForm] CreateUserDto dto)
        {
            if (!ModelState.IsValid) return BadRequest();

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

            return Created();
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update an existing user", Description = "Updates the information and profile image of a user")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUser(string id, [FromForm] CreateUserDto dto)
        {
            if (!ModelState.IsValid) return BadRequest();

            var user = _accountServiceForWebApi.GetUserById(id);
            if (user == null)
                return BadRequest("There is no account registered with this user");

            var save = new SaveUserDto
            {
                Id = id,
                Email = dto.Email,
                LastName = dto.LastName,
                Name = dto.Name,
                Password = dto.Password,
                Phone = dto.Phone,
                Role = dto.Role.ToString(),
                UserName = dto.UserName,
                ProfileImage = FileHandler.Upload(dto.ProfileImage, id, "Users")
            };

            var result = await _accountServiceForWebApi.EditUser(save, null, false, true);
            if (result == null || result.HasError)
                return BadRequest(result?.Errors);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete a user", Description = "Deletes a user by ID and removes their profile image")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(string id)
        {
            if (!ModelState.IsValid) return BadRequest();

            var user = await _accountServiceForWebApi.GetUserById(id);
            if (user == null) return BadRequest();

            var result = await _accountServiceForWebApi.DeleteAsync(id);
            if (result == null || result.HasError)
                return StatusCode(500, "Delete failed");

            FileHandler.Delete(id, "Users");

            return NoContent();
        }
    }
}

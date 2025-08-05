using Asp.Versioning;
using InvestmentApp.Core.Application.Dtos.InvestmentPortfolio;
using InvestmentApp.Core.Application.Features.InvestmentPortfolios.Commands.CreateInvestmentPortfolio;
using InvestmentApp.Core.Application.Features.InvestmentPortfolios.Commands.DeleteInvestmentPortfolio;
using InvestmentApp.Core.Application.Features.InvestmentPortfolios.Commands.UpdateInvestmentPortfolio;
using InvestmentApp.Core.Application.Features.InvestmentPortfolios.Queries.GetAllWithIncludeByUser;
using InvestmentApp.Core.Application.Features.InvestmentPortfolios.Queries.GetById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InvestmentApi.Controllers.v1
{
    [ApiVersion("1.0")]
    [Authorize]
    [SwaggerTag("CRUD operations for managing investment portfolios by the authenticated user")]
    public class InvestmentPortfolioController : BaseApiController
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<InvestmentPortfolioDto>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Get all investment portfolios",
            Description = "Retrieves all investment portfolios for the currently authenticated user"
        )]
        public async Task<IActionResult> Get()
        {
            var userId = User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(userId)) return BadRequest();

            var portfolios = await Mediator.Send(new GetAllInvestmentPortFolioWithIncludeByUserQuery() { UserId = userId });

            if (portfolios == null || portfolios.Count == 0)
                return NoContent();

            return Ok(portfolios);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InvestmentPortfolioDto))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Get investment portfolio by ID",
            Description = "Retrieves a specific investment portfolio by its ID for the authenticated user"
        )]
        public async Task<IActionResult> Get(int id)
        {
            var userId = User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(userId)) return BadRequest();

            var portfolio = await Mediator.Send(new GetByIdInvestmentPortFolioQuery { Id = id, UserId = userId });

            if (portfolio == null)
                return NoContent();

            if (portfolio.UserId != userId)
                return BadRequest();

            return Ok(portfolio);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Create new investment portfolio",
            Description = "Creates a new investment portfolio for the authenticated user"
        )]
        public async Task<IActionResult> Create([FromBody] CreateInvestmentPortfolioDto dto)
        {         
            var userId = User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(userId)) return BadRequest();

            var result = await Mediator.Send(new CreateInvestmentPortfolioCommand
            {
                Name = dto.Name,
                Description = dto.Description,
                UserId = userId
            });

            if (result == 0)
                return StatusCode(StatusCodes.Status500InternalServerError, "Creation failed");

            return Created();
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Update existing investment portfolio",
            Description = "Updates the details of an investment portfolio owned by the authenticated user"
        )]
        public async Task<IActionResult> Update(int id, [FromBody] CreateInvestmentPortfolioDto dto)
        {        
            var userId = User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(userId)) return BadRequest();

            var portfolio = await Mediator.Send(new GetByIdInvestmentPortFolioQuery
            {
                Id = id,
                UserId = userId
            });

            if (portfolio == null || portfolio.UserId != userId)
                return BadRequest();

            await Mediator.Send(new UpdateInvestmentPortfolioCommand
            {
                Id = id,
                Name = dto.Name,
                Description = dto.Description,
                UserId = userId
            });

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Delete investment portfolio",
            Description = "Deletes a portfolio belonging to the authenticated user by ID"
        )]
        public async Task<IActionResult> Delete(int id)
        {   
            var userId = User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(userId)) return BadRequest();

            var portfolio = await Mediator.Send(new GetByIdInvestmentPortFolioQuery
            {
                Id = id,
                UserId = userId
            });

            if (portfolio == null || portfolio.UserId != userId)
                return BadRequest();

            await Mediator.Send(new DeleteInvestmentPortfolioCommand { Id = id });

            return NoContent();
        }
    }
}

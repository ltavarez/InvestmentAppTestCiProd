using Asp.Versioning;
using InvestmentApp.Core.Application.Dtos.InvestmentAssets;
using InvestmentApp.Core.Application.Features.InvestmentAssets.Commands.CreateInvestmentAsset;
using InvestmentApp.Core.Application.Features.InvestmentAssets.Commands.DeleteInvestmentAsset;
using InvestmentApp.Core.Application.Features.InvestmentAssets.Commands.UpdateInvestmentAsset;
using InvestmentApp.Core.Application.Features.InvestmentAssets.Queries.GetAllWithInclude;
using InvestmentApp.Core.Application.Features.InvestmentAssets.Queries.GetByAssetAndPortfolio;
using InvestmentApp.Core.Application.Features.InvestmentAssets.Queries.GetById;
using InvestmentApp.Core.Application.Features.InvestmentPortfolios.Queries.GetById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InvestmentApi.Controllers.v1
{
    [ApiVersion("1.0")]
    [Authorize]
    [SwaggerTag("Manages investment assets related to user portfolios")]
    public class InvestmentAssetController : BaseApiController
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<InvestmentAssetsDto>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Retrieve all investment assets for the current user",
            Description = "Returns all investment assets associated with the authenticated user's portfolios"
        )]
        public async Task<IActionResult> Get()
        {
            var userId = User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(userId))
                return BadRequest();

            var investmentAssets = await Mediator.Send(new GetAllWithIncludeInvestmentAssetQuery { UserId = userId });

            if (investmentAssets == null || investmentAssets.Count == 0)
                return NoContent();

            return Ok(investmentAssets);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InvestmentAssetsDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Retrieve investment asset by ID",
            Description = "Returns a single investment asset record by ID, validating user ownership"
        )]
        public async Task<IActionResult> Get(int id)
        {
            var userId = User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(userId))
                return BadRequest();

            var investmentAsset = await Mediator.Send(new GetByIdInvestmentAssetQuery { Id = id, UserId = userId });

            if (investmentAsset == null)
                return NotFound();

            return Ok(investmentAsset);
        }

        [HttpGet("Portfolio")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InvestmentAssetsDto))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Retrieve investment asset by portfolio and asset",
            Description = "Returns a specific investment asset linked to a portfolio and asset ID, validating user"
        )]
        public async Task<IActionResult> GetByAssetAndPortfolioAsync(int assetId, int portfolioId)
        {
            var userId = User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(userId))
                return BadRequest();

            var investmentAsset = await Mediator.Send(new GetByAssetAndPortfolioInvestmentAssetQuery
            {
                AssetId = assetId,
                PortfolioId = portfolioId,
                UserId = userId
            });

            if (investmentAsset == null)
                return NoContent();

            return Ok(investmentAsset);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Create a new investment asset",
            Description = "Creates an investment asset after verifying ownership of the specified investment portfolio"
        )]
        public async Task<IActionResult> Create([FromBody] CreateInvestmentAssetCommand command)
        {
            var userId = User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(userId))
                return BadRequest();

            var investmentPortfolio = await Mediator.Send(new GetByIdInvestmentPortFolioQuery
            {
                Id = command.InvestmentPortfolioId,
                UserId = userId
            });

            if (investmentPortfolio == null)
                return NotFound();

            if (investmentPortfolio.UserId != userId)
                return BadRequest();

            var result = await Mediator.Send(command);

            if (result == 0)
                return StatusCode(StatusCodes.Status500InternalServerError, "Creation failed");

            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Update an existing investment asset",
            Description = "Updates an investment asset only if it belongs to the authenticated user"
        )]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateInvestmentAssetCommand command)
        {
            var userId = User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(userId))
                return BadRequest();

            var investmentAsset = await Mediator.Send(new GetByIdInvestmentAssetQuery { Id = id, UserId = userId });

            if (investmentAsset == null)
                return NotFound();

            await Mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Delete an investment asset",
            Description = "Deletes the specified investment asset only if it belongs to the authenticated user"
        )]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(userId))
                return BadRequest();

            var investmentAsset = await Mediator.Send(new GetByIdInvestmentAssetQuery { Id = id, UserId = userId });

            if (investmentAsset == null)
                return NotFound();

            await Mediator.Send(new DeleteInvestmentAssetCommand { Id = id });

            return NoContent();
        }
    }
}

using Asp.Versioning;
using InvestmentApp.Core.Application.Dtos.Asset;
using InvestmentApp.Core.Application.Features.Assets.Commands.CreateAsset;
using InvestmentApp.Core.Application.Features.Assets.Commands.DeleteAsset;
using InvestmentApp.Core.Application.Features.Assets.Commands.UpdateAsset;
using InvestmentApp.Core.Application.Features.Assets.Queries.GetAllAssetsByPortfolioId;
using InvestmentApp.Core.Application.Features.Assets.Queries.GetAllWithInclude;
using InvestmentApp.Core.Application.Features.Assets.Queries.GetById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InvestmentApi.Controllers.v1
{
    [ApiVersion("1.0")]
    [Authorize(Roles = "Admin")]
    [SwaggerTag("Provides CRUD operations and queries for managing investment assets")]
    public class AssetController : BaseApiController
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AssetDto>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Retrieve all assets",
            Description = "Returns a list of all assets including related entities like asset type or portfolio"
        )]
        public async Task<IActionResult> Get()
        {
            var assets = await Mediator.Send(new GetAllWithIncludeAssetQuery());

            if (assets == null || assets.Count == 0)
            {
                return NoContent();
            }

            return Ok(assets);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AssetDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Retrieve asset by ID",
            Description = "Returns the details of a specific asset using its unique identifier"
        )]
        public async Task<IActionResult> Get(int id)
        {
            var asset = await Mediator.Send(new GetByIdAssetQuery { Id = id });

            if (asset == null)
            {
                return NotFound();
            }

            return Ok(asset);
        }

        [HttpGet("Porfolio")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AssetForPortfolioDto>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Retrieve assets by portfolio",
            Description = "Returns a filtered list of assets that belong to a specific portfolio. Supports filtering by name, type, and order."
        )]
        public async Task<IActionResult> GetAssetsForPortfolio([FromQuery] AssetPortFolioRequestDto assetPortFolioRequestDto)
        {          
            var assets = await Mediator.Send(new GetAllAssetsByPortfolioIdQuery
            {
                PortfolioId = assetPortFolioRequestDto.PortfolioId,
                AssetName = assetPortFolioRequestDto.AssetName,
                AssetTypeId = assetPortFolioRequestDto.AssetTypeId,
                AssetOrderBy = assetPortFolioRequestDto.AssetOrderBy != null
                    ? (int)assetPortFolioRequestDto.AssetOrderBy
                    : 0
            });

            if (assets == null || assets.Count == 0)
            {
                return NoContent();
            }

            return Ok(assets);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Create a new asset",
            Description = "Creates a new investment asset using the provided data"
        )]
        public async Task<IActionResult> Create([FromBody] CreateAssetCommand command)
        {
            await Mediator.Send(command);
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Update an existing asset",
            Description = "Updates the specified asset with the new provided data"
        )]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAssetCommand command)
        {  
            if (id != command.Id)
            {
                return BadRequest("The ID in the URL does not match the request body.");
            }

            await Mediator.Send(command);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Delete an asset",
            Description = "Deletes the asset associated with the specified ID"
        )]
        public async Task<IActionResult> Delete(int id)
        {          
            await Mediator.Send(new DeleteAssetCommand { Id = id });

            return NoContent();
        }
    }
}

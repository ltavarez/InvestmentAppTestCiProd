using Asp.Versioning;
using InvestmentApp.Core.Application.Dtos.AssetHistory;
using InvestmentApp.Core.Application.Features.AssetsHistories.Commands.CreateAssetHistory;
using InvestmentApp.Core.Application.Features.AssetsHistories.Commands.DeleteAssetHistory;
using InvestmentApp.Core.Application.Features.AssetsHistories.Commands.UpdateAssetHistory;
using InvestmentApp.Core.Application.Features.AssetsHistories.Queries.GetAllWithInclude;
using InvestmentApp.Core.Application.Features.AssetsHistories.Queries.GetById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InvestmentApi.Controllers.v1
{
    [ApiVersion("1.0")]
    [Authorize(Roles = "Admin")]
    [SwaggerTag("Manages asset history records, including creation, updates, retrieval, and deletion")]
    public class AssetHistoryController : BaseApiController
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AssetHistoryDto>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Retrieve all asset history records",
            Description = "Returns a list of all asset histories with related asset and portfolio data"
        )]
        public async Task<IActionResult> Get()
        {
            var assetHistories = await Mediator.Send(new GetAllWithIncludeAssetHistoryQuery());

            if (assetHistories == null || assetHistories.Count == 0)
            {
                return NoContent();
            }

            return Ok(assetHistories);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AssetHistoryDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Retrieve asset history by ID",
            Description = "Returns the asset history record that matches the specified ID"
        )]
        public async Task<IActionResult> Get(int id)
        {
            var assetHistory = await Mediator.Send(new GetByIdAssetHistoryQuery { Id = id });

            if (assetHistory == null)
            {
                return NotFound();
            }

            return Ok(assetHistory);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Create a new asset history record",
            Description = "Registers a new asset history entry based on the provided data"
        )]
        public async Task<IActionResult> Create([FromBody] CreateAssetHistoryCommand command)
        {
            var result = await Mediator.Send(command);

            if (result == 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Creation failed");
            }

            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Update an existing asset history record",
            Description = "Updates the specified asset history record with new information"
        )]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAssetHistoryCommand command)
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
            Summary = "Delete an asset history record",
            Description = "Deletes the asset history record that matches the specified ID"
        )]
        public async Task<IActionResult> Delete(int id)
        {            
            await Mediator.Send(new DeleteAssetHistoryCommand { Id = id });

            return NoContent();
        }
    }
}

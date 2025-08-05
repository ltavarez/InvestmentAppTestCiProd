using Asp.Versioning;
using InvestmentApp.Core.Application.Dtos.AssetType;
using InvestmentApp.Core.Application.Features.AssetType.Commands.CreateAssetType;
using InvestmentApp.Core.Application.Features.AssetType.Commands.DeleteAssetType;
using InvestmentApp.Core.Application.Features.AssetType.Commands.UpdateAssetType;
using InvestmentApp.Core.Application.Features.AssetType.Queries.GetAllWithInclude;
using InvestmentApp.Core.Application.Features.AssetType.Queries.GetById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;

namespace InvestmentApi.Controllers.v1
{
    [ApiVersion("1.0")]
    [Authorize(Roles = "Admin")]
    [SwaggerTag("Provides endpoints for managing asset types (CRUD operations)")]
    public class AssetTypeController : BaseApiController
    {
        [HttpGet]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AssetTypeDto>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Retrieve all asset types",
            Description = "Returns a list of all asset types with related entities included"
        )]
        public async Task<IActionResult> Get()
        {
            var assetTypes = await Mediator.Send(new GetAllWithIncludeAssetTypeQuery());

            if (assetTypes == null || assetTypes.Count == 0)
            {
                return NoContent();
            }

            return Ok(assetTypes);
        }

        [HttpGet("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AssetTypeDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Retrieve asset type by ID",
            Description = "Returns the asset type that matches the provided identifier"
        )]
        public async Task<IActionResult> Get(int id)
        {
            var assetType = await Mediator.Send(new GetByIdAssetTypeQuery { Id = id });

            return Ok(assetType);
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Create a new asset type",
            Description = "Creates a new asset type with the provided data"
        )]
        public async Task<IActionResult> Create([FromBody] CreateAssetTypeCommand command)
        {  
            await Mediator.Send(command);     
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPut("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Update an existing asset type",
            Description = "Updates the asset type that matches the given ID with the provided data"
        )]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAssetTypeCommand command)
        { 
            if (id != command.Id)
            {
                return BadRequest("The ID in the URL does not match the request body.");
            }

            await Mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Delete an asset type",
            Description = "Deletes the asset type that matches the specified ID"
        )]
        public async Task<IActionResult> Delete(int id)
        {     
            await Mediator.Send(new DeleteAssetTypeCommand { Id = id });
            return NoContent();
        }
    }
}

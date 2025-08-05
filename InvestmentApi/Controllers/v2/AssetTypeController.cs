using Asp.Versioning;
using InvestmentApp.Core.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InvestmentApi.Controllers.v2
{
    /// <summary>
    /// Controller for managing asset types (v2).
    /// </summary>
    /// <remarks>
    /// Provides endpoints to retrieve asset types with related data.
    /// </remarks>
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AssetTypeController(IAssetTypeService assetTypeService) : BaseApiController
    {
        private readonly IAssetTypeService _assetTypeService = assetTypeService;

        /// <summary>
        /// Retrieves all asset types with their related entities.
        /// </summary>
        /// <remarks>
        /// This endpoint returns all asset types including additional related information 
        /// such as assets or classifications, depending on service implementation.
        /// </remarks>
        /// <returns>List of asset types with related data</returns>
        /// <response code="200">Returns the list of asset types</response>
        /// <response code="204">No asset types were found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            var assetTypes = await _assetTypeService.GetAllWithInclude();

            if (assetTypes == null || assetTypes.Count == 0)
            {
                return NoContent();
            }

            return Ok(assetTypes);
        }
    }
}

using InvestmentApp.Core.Domain.Common.Enums;
using Swashbuckle.AspNetCore.Annotations;

namespace InvestmentApp.Core.Application.Dtos.Asset
{
    /// <summary>
    /// Query parameters used to filter and order assets within a specific portfolio
    /// </summary>
    public class AssetPortFolioRequestDto
    {
        /// <example>12</example>
        [SwaggerParameter(Description = "The ID of the portfolio to retrieve assets for")]
        public required int PortfolioId { get; set; }

        /// <example>Bitcoin</example>
        [SwaggerParameter(Description = "Optional filter by asset name")]
        public string? AssetName { get; set; } = null;

        /// <example>3</example>
        [SwaggerParameter(Description = "Optional filter by asset type ID")]
        public int? AssetTypeId { get; set; } = null;

        /// <example>ValueDesc</example>
        [SwaggerParameter(Description = "Optional order in which to return the assets (e.g., NameAsc, ValueDesc)")]
        public AssetOrdered? AssetOrderBy { get; set; } = null;
    }

}

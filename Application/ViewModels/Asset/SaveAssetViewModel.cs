using System.ComponentModel.DataAnnotations;

namespace InvestmentApp.Core.Application.ViewModels.Asset
{
    public class SaveAssetViewModel
    {      
        public int Id { get; set; }

        [Required(ErrorMessage = "You must enter the name of asset")]
        public required string Name { get; set; }
        public string? Description { get; set; }

        [Required(ErrorMessage = "You must enter the symbol of asset")]
        public required string Symbol { get; set; }

        [Range(1, int.MaxValue , ErrorMessage = "You must enter the valid type of asset")]
        public int? AssetTypeId { get; set; }
    }
}

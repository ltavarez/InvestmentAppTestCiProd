using System.ComponentModel.DataAnnotations;

namespace InvestmentApp.Core.Application.ViewModels.AssetType
{
    public class SaveAssetTypeViewModel
    {      
        public int Id { get; set; }

        [Required(ErrorMessage = "You must enter the name of asset type")]
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}

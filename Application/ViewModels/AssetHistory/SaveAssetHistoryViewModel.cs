using System.ComponentModel.DataAnnotations;

namespace InvestmentApp.Core.Application.ViewModels.AssetHistory
{
    public class SaveAssetHistoryViewModel
    {
        public required int Id { get; set; }

        [Required(ErrorMessage = "You must enter the date of history")]
        public DateTime HistoryValueDate { get; set; }

        [Required(ErrorMessage = "You must enter the value of history")]
        public required decimal Value { get; set; }
        public required int AssetId { get; set; }       
    }
}

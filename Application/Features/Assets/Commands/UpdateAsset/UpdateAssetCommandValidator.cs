using FluentValidation;
using InvestmentApp.Core.Domain.Interfaces;

namespace InvestmentApp.Core.Application.Features.Assets.Commands.UpdateAsset
{
    public class UpdateAssetCommandValidator : AbstractValidator<UpdateAssetCommand>
    {
        private readonly IAssetTypeRepository _assetTypeRepository;   
        public UpdateAssetCommandValidator(IAssetTypeRepository assetTypeRepository)
        {
            _assetTypeRepository = assetTypeRepository;

            RuleFor(a => a.Id)
                .GreaterThan(0).WithMessage("Asset ID must be greater than 0.")
                .NotNull().WithMessage("Asset ID is required.");           

            RuleFor(a => a.Name)
                .NotEmpty().WithMessage("Asset type name is required.")
                .MaximumLength(150).WithMessage("Asset type name must not exceed 150 characters.");

            RuleFor(a => a.Description)
                .MaximumLength(250).WithMessage("Asset type description must not exceed 250 characters.");

            RuleFor(a => a.Symbol).
                NotEmpty().WithMessage("Asset symbol is required.")
                .MaximumLength(10).WithMessage("Asset symbol must not exceed 10 characters.")
                .Matches("^[A-Z]+$").WithMessage("Asset symbol must consist of uppercase letters only.");

            RuleFor(a => a.AssetTypeId)
                .GreaterThan(0).WithMessage("Asset type ID must be greater than 0.")
                .WithMessage("Asset type ID is required.")
                .MustAsync(async (id, cancellation) => 
                {
                    // Assuming you have a method to check if the AssetType exists
                    var asstTypeExists = await _assetTypeRepository!.GetById(id);
                    return asstTypeExists != null;  
                }).WithMessage("Asset type does not exist.");
        }
    }
}

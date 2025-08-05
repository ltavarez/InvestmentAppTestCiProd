using AutoMapper;
using InvestmentApp.Core.Application.Dtos.InvestmentAssets;
using InvestmentApp.Core.Application.Exceptions;
using InvestmentApp.Core.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace InvestmentApp.Core.Application.Features.InvestmentAssets.Queries.GetByAssetAndPortfolio
{
    public class GetByAssetAndPortfolioInvestmentAssetQuery : IRequest<InvestmentAssetsDto>
    {
        public int AssetId { get; set; }
        public int PortfolioId { get; set; }        
        public string? UserId { get; set; }
    }
    public class GetByAssetAndPortfolioInvestmentAssetQueryHandler : IRequestHandler<GetByAssetAndPortfolioInvestmentAssetQuery, InvestmentAssetsDto>
    {
        private readonly IInvestmentAssetRepository _investmentAssetRepository;
        private readonly IMapper _mapper;

        public GetByAssetAndPortfolioInvestmentAssetQueryHandler(IInvestmentAssetRepository investmentAssetRepository, IMapper mapper)
        {
            _investmentAssetRepository = investmentAssetRepository;
            _mapper = mapper;
        }
        public async Task<InvestmentAssetsDto> Handle(GetByAssetAndPortfolioInvestmentAssetQuery query, CancellationToken cancellationToken)
        {
            var entity = await _investmentAssetRepository
                .GetAllQueryWithInclude(["Asset"])
                .FirstOrDefaultAsync(ia => ia.AssetId == query.AssetId
                && ia.InvestmentPortfolioId == query.PortfolioId
                && ia.InvestmentPortfolio != null
                && ia.InvestmentPortfolio.UserId == query.UserId);

            if (entity == null) throw new ApiException("Investment Assets not found with this id", (int)HttpStatusCode.NotFound);
        
            InvestmentAssetsDto dto = _mapper.Map<InvestmentAssetsDto>(entity);

            return dto;
        }
    }
}

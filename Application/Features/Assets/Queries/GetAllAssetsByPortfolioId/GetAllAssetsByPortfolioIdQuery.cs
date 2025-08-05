using AutoMapper;
using AutoMapper.QueryableExtensions;
using InvestmentApp.Core.Application.Dtos.Asset;
using InvestmentApp.Core.Domain.Common.Enums;
using InvestmentApp.Core.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Core.Application.Features.Assets.Queries.GetAllAssetsByPortfolioId
{
    public class GetAllAssetsByPortfolioIdQuery : IRequest<IList<AssetForPortfolioDto>>
    {
        public required int PortfolioId { get; set; }
        public string? AssetName { get; set; }
        public int? AssetTypeId { get; set; }
        public int? AssetOrderBy { get; set; }
    }

    public class GetAllAssetsByPortfolioIdQueryHandler : IRequestHandler<GetAllAssetsByPortfolioIdQuery, IList<AssetForPortfolioDto>>
    {
        private readonly IAssetRepository _assetRepository;
        private readonly IInvestmentAssetRepository _investmentAssetRepository;
        private readonly IMapper _mapper;

        public GetAllAssetsByPortfolioIdQueryHandler(IAssetRepository assetRepository, IMapper mapper, IInvestmentAssetRepository investmentAssetRepository)
        {
            _assetRepository = assetRepository;
            _mapper = mapper;
            _investmentAssetRepository = investmentAssetRepository;
        }

        public async Task<IList<AssetForPortfolioDto>> Handle(GetAllAssetsByPortfolioIdQuery query, CancellationToken cancellationToken)
        {
            var assetIds = await _investmentAssetRepository
                   .GetAllQuery()
                   .Where(ia => ia.InvestmentPortfolioId == query.PortfolioId)
                   .Select(s => s.AssetId).ToListAsync();

            if (assetIds.Count == 0)
            {
                return [];
            }

            var listEntitiesQuery = _assetRepository
                .GetAllQueryWithInclude(["AssetType", "AssetHistories"])
                .Where(w => assetIds.Contains(w.Id));

            var listEntityDtos = listEntitiesQuery.ProjectTo<AssetForPortfolioDto>(_mapper.ConfigurationProvider);

            if (!string.IsNullOrWhiteSpace(query.AssetName))
            {
                listEntityDtos = listEntityDtos.Where(w => w.Name.Contains(query.AssetName));
            }

            if (query.AssetTypeId.HasValue)
            {
                listEntityDtos = listEntityDtos.Where(w => w.AssetTypeId == query.AssetTypeId);
            }

            var listDtos = await listEntityDtos.ToListAsync();

            if (query.AssetOrderBy.HasValue)
            {
                var listOrderDtos = query.AssetOrderBy switch
                {
                    (int)AssetOrdered.BY_NAME => listDtos.OrderBy(o => o.Name),
                    (int)AssetOrdered.BY_CURRENT_VALUE => listDtos.OrderByDescending(o => o.CurrentValue),
                    _ => listDtos.OrderBy(o => o.Name),
                };

                listDtos = listOrderDtos.ToList();
            }
            else
            {
                listDtos = listDtos.OrderBy(o => o.Name).ToList();
            }

            return listDtos;
        }
    }
}

using AutoMapper;
using AutoMapper.QueryableExtensions;
using InvestmentApp.Core.Application.Dtos.InvestmentAssets;
using InvestmentApp.Core.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Core.Application.Features.InvestmentAssets.Queries.GetAllWithInclude
{
    public class GetAllWithIncludeInvestmentAssetQuery : IRequest<IList<InvestmentAssetsDto>>
    {
        public required string UserId { get; set; }
    }
    public class GetAllWithIncludeInvestmentAssetQueryHandler : IRequestHandler<GetAllWithIncludeInvestmentAssetQuery, IList<InvestmentAssetsDto>>
    {
        private readonly IInvestmentAssetRepository _investmentAssetRepository;
        private readonly IMapper _mapper;

        public GetAllWithIncludeInvestmentAssetQueryHandler(IInvestmentAssetRepository investmentAssetRepository, IMapper mapper)
        {
            _investmentAssetRepository = investmentAssetRepository;
            _mapper = mapper;
        }
        public async Task<IList<InvestmentAssetsDto>> Handle(GetAllWithIncludeInvestmentAssetQuery query, CancellationToken cancellationToken)
        {
            var listEntitiesQuery = _investmentAssetRepository.GetAllQueryWithInclude(["Asset", "InvestmentPortfolio"]);
            listEntitiesQuery = listEntitiesQuery
    .Where(w => w.InvestmentPortfolio != null && w.InvestmentPortfolio.UserId == query.UserId);

            var listEntityDtos = await listEntitiesQuery.ProjectTo<InvestmentAssetsDto>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken: cancellationToken);

            return listEntityDtos;
        }
    }
}

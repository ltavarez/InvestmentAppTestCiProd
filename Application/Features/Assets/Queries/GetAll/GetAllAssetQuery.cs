using AutoMapper;
using InvestmentApp.Core.Application.Dtos.Asset;
using InvestmentApp.Core.Application.Dtos.AssetType;
using InvestmentApp.Core.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Core.Application.Features.Assets.Queries.GetAll
{
    public class GetAllAssetQuery : IRequest<IList<AssetWithTypeDto>>
    {
    }
    public class GetAllAssetQueryHandler : IRequestHandler<GetAllAssetQuery, IList<AssetWithTypeDto>>
    {
        private readonly IAssetRepository _assetRepository;
        private readonly IMapper _mapper;

        public GetAllAssetQueryHandler(IAssetRepository assetRepository, IMapper mapper)
        {
            _assetRepository = assetRepository;
            _mapper = mapper;
        }

        public async Task<IList<AssetWithTypeDto>> Handle(GetAllAssetQuery query, CancellationToken cancellationToken)
        {
            var listEntitiesQuery = _assetRepository.GetAllQueryWithInclude(["AssetType"]);

            var listEntityDtos = await listEntitiesQuery.Select(s =>

                new AssetWithTypeDto()
                {
                    AssetTypeId = s.AssetTypeId,
                    Id = s.Id,
                    Name = s.Name,
                    Symbol = s.Symbol,
                    AssetType = s.AssetType != null
                                ? _mapper.Map<AssetTypeDto>(s.AssetType) : null
                }
            ).ToListAsync();

            return listEntityDtos;
        }
    }
}

using AutoMapper;
using InvestmentApp.Core.Application.Dtos.Asset;
using InvestmentApp.Core.Application.Exceptions;
using InvestmentApp.Core.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace InvestmentApp.Core.Application.Features.Assets.Queries.GetById
{
    public class GetByIdAssetQuery : IRequest<AssetDto>
    {
        public required int Id { get; set; }
    }

    public class GetByIdAssetQueryHandler : IRequestHandler<GetByIdAssetQuery, AssetDto>
    {
        private readonly IAssetRepository _assetRepository;
        private readonly IMapper _mapper;

        public GetByIdAssetQueryHandler(IAssetRepository assetRepository, IMapper mapper)
        {
            _assetRepository = assetRepository;
            _mapper = mapper;
        }

        public async Task<AssetDto> Handle(GetByIdAssetQuery query, CancellationToken cancellationToken)
        {
            var listEntitiesQuery = _assetRepository.GetAllQueryWithInclude(["AssetType", "AssetHistories"]);
            Domain.Entities.Asset? entity = await listEntitiesQuery.FirstOrDefaultAsync(fd => fd.Id == query.Id, cancellationToken: cancellationToken);

            if (entity == null) throw new ApiException("Asset not found with this id", (int)HttpStatusCode.NotFound);

            var dto = _mapper.Map<AssetDto>(entity);

            return dto;
        }
    }
}

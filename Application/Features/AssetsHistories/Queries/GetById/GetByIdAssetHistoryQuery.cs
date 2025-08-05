using AutoMapper;
using InvestmentApp.Core.Application.Dtos.AssetHistory;
using InvestmentApp.Core.Application.Exceptions;
using InvestmentApp.Core.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace InvestmentApp.Core.Application.Features.AssetsHistories.Queries.GetById
{
    public class GetByIdAssetHistoryQuery : IRequest<AssetHistoryDto>
    {       
        public int Id { get; set; }
    }
    public class GetByIdAssetHistoryQueryHandler : IRequestHandler<GetByIdAssetHistoryQuery, AssetHistoryDto>
    {
        private readonly IAssetHistoryRepository _assetHistoryRepository;
        private readonly IMapper _mapper;
        public GetByIdAssetHistoryQueryHandler(IAssetHistoryRepository assetHistoryRepository, IMapper mapper)
        {
            _assetHistoryRepository = assetHistoryRepository;
            _mapper = mapper;
        }
        public async Task<AssetHistoryDto> Handle(GetByIdAssetHistoryQuery query, CancellationToken cancellationToken)
        {
            var listEntitiesQuery = _assetHistoryRepository.GetAllQueryWithInclude(["Asset"]);
            Domain.Entities.AssetHistory? entity = await listEntitiesQuery.FirstOrDefaultAsync(fd => fd.Id == query.Id, cancellationToken: cancellationToken);

            if (entity == null) throw new ApiException("Asset history not found with this id", (int)HttpStatusCode.NotFound);

            var dto = _mapper.Map<AssetHistoryDto>(entity);

            return dto;
        }
    }
}

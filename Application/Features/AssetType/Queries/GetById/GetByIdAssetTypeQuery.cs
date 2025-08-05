using AutoMapper;
using InvestmentApp.Core.Application.Dtos.Asset;
using InvestmentApp.Core.Application.Dtos.AssetType;
using InvestmentApp.Core.Application.Exceptions;
using InvestmentApp.Core.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace InvestmentApp.Core.Application.Features.AssetType.Queries.GetById
{
    /// <summary>
    /// Query parameters for retrieving a single asset type by its unique identifier
    /// </summary>
    public class GetByIdAssetTypeQuery : IRequest<AssetTypeResponseDto>
    {
        /// <example>5</example>
        [SwaggerParameter(Description = "The unique identifier of the asset type")]
        public int Id { get; set; }
    }

    public class GetByIdAssetTypeQueryHandler : IRequestHandler<GetByIdAssetTypeQuery, AssetTypeResponseDto>
    {
        private readonly IAssetTypeRepository _assetTypeRepository;
        private readonly IMapper _mapper;

        public GetByIdAssetTypeQueryHandler(IAssetTypeRepository assetTypeRepository, IMapper mapper)
        {
            _assetTypeRepository = assetTypeRepository;
            _mapper = mapper;
        }

        public async Task<AssetTypeResponseDto> Handle(GetByIdAssetTypeQuery query, CancellationToken cancellationToken)
        {
            var listEntitiesQuery = _assetTypeRepository.GetAllQueryWithInclude(["Assets"]);
            Domain.Entities.AssetType? entity = await listEntitiesQuery.FirstOrDefaultAsync(fd => fd.Id == query.Id, cancellationToken: cancellationToken);

            if (entity == null) throw new ApiException("Asset type not found with this id", (int)HttpStatusCode.NotFound);

            AssetTypeResponseDto response = new()
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Assets = entity.Assets != null && entity.Assets.Count > 0
                ? _mapper.Map<List<AssetBasicDto>>(entity.Assets)
                : []
            };

            return response;
        }
    }
}

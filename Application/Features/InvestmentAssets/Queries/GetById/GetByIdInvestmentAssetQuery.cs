using AutoMapper;
using InvestmentApp.Core.Application.Dtos.InvestmentAssets;
using InvestmentApp.Core.Application.Exceptions;
using InvestmentApp.Core.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace InvestmentApp.Core.Application.Features.InvestmentAssets.Queries.GetById
{
    public class GetByIdInvestmentAssetQuery : IRequest<InvestmentAssetsDto>
    {       
        public int Id { get; set; }
        public string? UserId { get; set; }
    }
    public class GetByIdInvestmentAssetQueryHandler(IInvestmentAssetRepository investmentAssetRepository, IMapper mapper) : IRequestHandler<GetByIdInvestmentAssetQuery, InvestmentAssetsDto>
    {
        private readonly IInvestmentAssetRepository _investmentAssetRepository = investmentAssetRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<InvestmentAssetsDto> Handle(GetByIdInvestmentAssetQuery query, CancellationToken cancellationToken)
        {
            var listEntitiesQuery = _investmentAssetRepository.GetAllQueryWithInclude(["Asset", "InvestmentPortfolio"]);
            Domain.Entities.InvestmentAssets? entity = await listEntitiesQuery.FirstOrDefaultAsync(
                fd => fd.Id == query.Id 
                && fd.InvestmentPortfolio != null 
                && fd.InvestmentPortfolio.UserId == query.UserId, cancellationToken: cancellationToken);

            if (entity == null) throw new ApiException("Investment Assets not found with this id", (int)HttpStatusCode.NotFound);            

            var dto = _mapper.Map<InvestmentAssetsDto>(entity);

            return dto;
        }
    }
}

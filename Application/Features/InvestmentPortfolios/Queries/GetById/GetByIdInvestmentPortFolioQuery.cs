using AutoMapper;
using InvestmentApp.Core.Application.Dtos.InvestmentPortfolio;
using InvestmentApp.Core.Application.Exceptions;
using InvestmentApp.Core.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace InvestmentApp.Core.Application.Features.InvestmentPortfolios.Queries.GetById
{
    public class GetByIdInvestmentPortFolioQuery : IRequest<InvestmentPortfolioDto>
    {       
        public required int Id { get; set; }
        public required string UserId { get; set; }
    }
    public class GetByIdInvestmentPortFolioQueryHandler : IRequestHandler<GetByIdInvestmentPortFolioQuery, InvestmentPortfolioDto>
    {
        private readonly IInvestmentPortfolioRepository _investmentPortfolioRepository;
        private readonly IMapper _mapper;
        public GetByIdInvestmentPortFolioQueryHandler(IInvestmentPortfolioRepository investmentPortfolioRepository, IMapper mapper)
        {
            _investmentPortfolioRepository = investmentPortfolioRepository;
            _mapper = mapper;
        }
        public async Task<InvestmentPortfolioDto> Handle(GetByIdInvestmentPortFolioQuery query, CancellationToken cancellationToken)
        {
            var listEntitiesQuery = _investmentPortfolioRepository.GetAllQuery();
            Domain.Entities.InvestmentPortfolio? entity = await listEntitiesQuery.FirstOrDefaultAsync(fd => fd.Id == query.Id && fd.UserId == query.UserId, cancellationToken: cancellationToken);

            if (entity == null) throw new ApiException("Investment Portfolio not found with this id", (int)HttpStatusCode.NotFound);

            var dto = _mapper.Map<InvestmentPortfolioDto>(entity);

            return dto;
        }
    }
}

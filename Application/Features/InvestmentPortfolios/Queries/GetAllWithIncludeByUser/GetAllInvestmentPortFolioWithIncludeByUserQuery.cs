using AutoMapper;
using AutoMapper.QueryableExtensions;
using InvestmentApp.Core.Application.Dtos.InvestmentPortfolio;
using InvestmentApp.Core.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Core.Application.Features.InvestmentPortfolios.Queries.GetAllWithIncludeByUser
{
    public class GetAllInvestmentPortFolioWithIncludeByUserQuery : IRequest<IList<InvestmentPortfolioDto>>
    {
        public required string UserId { get; set; }
    }
    public class GetAllInvestmentPortFolioWithIncludeByUserQueryHandler : IRequestHandler<GetAllInvestmentPortFolioWithIncludeByUserQuery, IList<InvestmentPortfolioDto>>
    {
        private readonly IInvestmentPortfolioRepository _investmentPortfolioRepository;
        private readonly IMapper _mapper;
        public GetAllInvestmentPortFolioWithIncludeByUserQueryHandler(IInvestmentPortfolioRepository investmentPortfolioRepository, IMapper mapper)
        {
            _investmentPortfolioRepository = investmentPortfolioRepository;
            _mapper = mapper;
        }
        public async Task<IList<InvestmentPortfolioDto>> Handle(GetAllInvestmentPortFolioWithIncludeByUserQuery query, CancellationToken cancellationToken)
        {
            var listEntitiesQuery = _investmentPortfolioRepository.GetAllQuery()
                      .Where(ip => ip.UserId == query.UserId);

            var listEntityDtos = await listEntitiesQuery.ProjectTo<InvestmentPortfolioDto>(_mapper.ConfigurationProvider).ToListAsync();

            return listEntityDtos;
        }
    }
}

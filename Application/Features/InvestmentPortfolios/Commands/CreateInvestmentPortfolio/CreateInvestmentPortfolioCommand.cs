using AutoMapper;
using InvestmentApp.Core.Domain.Interfaces;
using MediatR;

namespace InvestmentApp.Core.Application.Features.InvestmentPortfolios.Commands.CreateInvestmentPortfolio
{
    public class CreateInvestmentPortfolioCommand : IRequest<int>
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? UserId { get; set; } // FK
    }
    public class CreateInvestmentPortfolioCommandHandler : IRequestHandler<CreateInvestmentPortfolioCommand, int>
    {
        private readonly IInvestmentPortfolioRepository _investmentPortfolioRepository;
        public CreateInvestmentPortfolioCommandHandler(IInvestmentPortfolioRepository investmentPortfolioRepository, IMapper mapper)
        {
            _investmentPortfolioRepository = investmentPortfolioRepository;            
        }
        public async Task<int> Handle(CreateInvestmentPortfolioCommand command, CancellationToken cancellationToken)
        {
            Domain.Entities.InvestmentPortfolio entity = new()
            {
                Id = 0,
                Name = command.Name ?? "",
                UserId = command.UserId ?? "",
                Description = command.Description,
            };

            Domain.Entities.InvestmentPortfolio? result = await _investmentPortfolioRepository.AddAsync(entity);

            return result != null ? result.Id : 0;
        }
    }
}

using AutoMapper;
using InvestmentApp.Core.Application.Exceptions;
using InvestmentApp.Core.Domain.Interfaces;
using MediatR;
using System.Net;

namespace InvestmentApp.Core.Application.Features.InvestmentPortfolios.Commands.DeleteInvestmentPortfolio
{
    public class DeleteInvestmentPortfolioCommand : IRequest<Unit>
    {
        public int Id { get; set; }
    }
    public class DeleteInvestmentPortfolioCommandHandler : IRequestHandler<DeleteInvestmentPortfolioCommand, Unit>
    {
        private readonly IInvestmentPortfolioRepository _investmentPortfolioRepository;
        public DeleteInvestmentPortfolioCommandHandler(IInvestmentPortfolioRepository investmentPortfolioRepository, IMapper mapper)
        {
            _investmentPortfolioRepository = investmentPortfolioRepository;
        }
        public async Task<Unit> Handle(DeleteInvestmentPortfolioCommand command, CancellationToken cancellationToken)
        {
            Domain.Entities.InvestmentPortfolio? entity = await _investmentPortfolioRepository.GetById(command.Id);
            if (entity == null) throw new ApiException("Investment Portfolio not found with this id", (int)HttpStatusCode.NotFound);

            await _investmentPortfolioRepository.DeleteAsync(command.Id);            

            return Unit.Value;
        }
    }
}

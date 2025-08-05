using AutoMapper;
using InvestmentApp.Core.Application.Exceptions;
using InvestmentApp.Core.Domain.Interfaces;
using MediatR;
using System.Net;

namespace InvestmentApp.Core.Application.Features.InvestmentPortfolios.Commands.UpdateInvestmentPortfolio
{
    public class UpdateInvestmentPortfolioCommand : IRequest<Unit>
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? UserId { get; set; } // FK
    }
    public class UpdateInvestmentPortfolioCommandHandler : IRequestHandler<UpdateInvestmentPortfolioCommand, Unit>
    {
        private readonly IInvestmentPortfolioRepository _investmentPortfolioRepository;
        public UpdateInvestmentPortfolioCommandHandler(IInvestmentPortfolioRepository investmentPortfolioRepository, IMapper mapper)
        {
            _investmentPortfolioRepository = investmentPortfolioRepository;
        }
        public async Task<Unit> Handle(UpdateInvestmentPortfolioCommand command, CancellationToken cancellationToken)
        {
            Domain.Entities.InvestmentPortfolio entity = new()
            {
                Id = command.Id,
                Name = command.Name ?? "",
                Description = command.Description,
                UserId = command.UserId ?? ""
            };

            Domain.Entities.InvestmentPortfolio? getEntity = await _investmentPortfolioRepository.GetById(command.Id);
            if (getEntity == null) throw new ApiException("Investment Portfolio not found with this id", (int)HttpStatusCode.NotFound);

            await _investmentPortfolioRepository.UpdateAsync(command.Id, entity);

            return Unit.Value;
        }
    }
}

using AutoMapper;
using AutoMapper.QueryableExtensions;
using InvestmentApp.Core.Application.Dtos.InvestmentPortfolio;
using InvestmentApp.Core.Application.Interfaces;
using InvestmentApp.Core.Domain.Entities;
using InvestmentApp.Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Core.Application.Services
{
    public class InvestmentPortfolioService : GenericService<InvestmentPortfolio, InvestmentPortfolioDto>, IInvestmentPortfolioService
    {
        private readonly IInvestmentPortfolioRepository _investmentPortfolioRepository;
        private readonly IMapper _mapper;

        public InvestmentPortfolioService(IInvestmentPortfolioRepository investmentPortfolioRepository, IMapper mapper) : base(investmentPortfolioRepository, mapper)
        {
            _investmentPortfolioRepository = investmentPortfolioRepository;
            _mapper = mapper;
        }       
        public override async Task<InvestmentPortfolioDto?> GetById(int id)
        {
            try
            {
                var listEntitiesQuery = _investmentPortfolioRepository.GetAllQuery();
                var entity = await listEntitiesQuery.FirstOrDefaultAsync(a => a.Id == id);


                if (entity == null)
                {
                    return null;
                }

                var dto = _mapper.Map<InvestmentPortfolioDto>(entity);
                return dto;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<InvestmentPortfolioDto?> GetById(int id,string userId)
        {
            try
            {
                var listEntitiesQuery = _investmentPortfolioRepository.GetAllQuery();
                var entity = await listEntitiesQuery.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

                if (entity == null)
                {
                    return null;
                }

                var dto = _mapper.Map<InvestmentPortfolioDto>(entity);
                return dto;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<InvestmentPortfolioDto>> GetAllWithIncludeByUser(string userId)
        {
            try
            {
                var listEntitiesQuery = _investmentPortfolioRepository.GetAllQuery()
                    .Where(ip=>ip.UserId == userId);

                var listEntityDtos = await listEntitiesQuery.ProjectTo<InvestmentPortfolioDto>(_mapper.ConfigurationProvider).ToListAsync();            

                return listEntityDtos;
            }
            catch (Exception)
            {
                return [];
            }
        }
    }
}
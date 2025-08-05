using AutoMapper;
using InvestmentApp.Core.Application.Dtos.InvestmentAssets;
using InvestmentApp.Core.Application.Interfaces;
using InvestmentApp.Core.Application.ViewModels.InvestmentAssets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ItlaInvestmentApp.Controllers
{
    [Authorize]
    public class InvestmentAssetController : Controller
    {
        private readonly IInvestmentAssetsService _investmentAssetsService;
        private readonly IInvestmentPortfolioService _investmentPortfolioService;
        private readonly IAssetService _assetService;
        private readonly IMapper _mapper;
        public InvestmentAssetController(IInvestmentAssetsService investmentAssetsService, IAssetService assetService, IInvestmentPortfolioService investmentPortfolioService, IMapper mapper)
        {
            _investmentAssetsService = investmentAssetsService;
            _investmentPortfolioService = investmentPortfolioService;
            _assetService = assetService;
            _mapper = mapper;
        }
        public async Task<IActionResult> Create(int portfolioId)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToRoute(new { controller = "InvestmentPortfolio", action = "AssetsDetails", portfolioId });
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToRoute(new { controller = "InvestmentPortfolio", action = "AssetsDetails", portfolioId });
            }

            var investmentPortfolio = await _investmentPortfolioService.GetById(portfolioId, userId);

            if (investmentPortfolio == null)
            {
                return RedirectToRoute(new { controller = "InvestmentPortfolio", action = "AssetsDetails", portfolioId });
            }

            ViewBag.Assets = await _assetService.GetAll();
            return View(new SaveInvestmentAssetViewModel() { AssetId = 0, Id = 0, InvestmentPortfolioId = portfolioId });
        }

        [HttpPost]
        public async Task<IActionResult> Create(SaveInvestmentAssetViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Assets = await _assetService.GetAll();
                return View(vm);
            }

            InvestmentAssetsDto dto = _mapper.Map<InvestmentAssetsDto>(vm);

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToRoute(new { controller = "InvestmentPortfolio", action = "AssetsDetails", vm.InvestmentPortfolioId });
            }

            var investmentPortfolio = await _investmentPortfolioService.GetById(vm.InvestmentPortfolioId, userId);

            if (investmentPortfolio == null)
            {
                return RedirectToRoute(new { controller = "InvestmentPortfolio", action = "AssetsDetails", vm.InvestmentPortfolioId });
            }

            await _investmentAssetsService.AddAsync(dto);
            return RedirectToRoute(new { controller = "InvestmentPortfolio", action = "AssetsDetails", portfolioId = vm.InvestmentPortfolioId });
        }

        public async Task<IActionResult> Delete(int assetId, int portfolioId)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToRoute(new { controller = "InvestmentPortfolio", action = "AssetsDetails", portfolioId });
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

            var dto = await _investmentAssetsService.GetByAssetAndPortfolioAsync(assetId, portfolioId, userId);
            if (dto == null)
            {
                return RedirectToRoute(new { controller = "InvestmentPortfolio", action = "AssetsDetails", portfolioId });
            }

            DeleteInvestmentAssetViewModel vm = _mapper.Map<DeleteInvestmentAssetViewModel>(dto);
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(DeleteInvestmentAssetViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToRoute(new { controller = "InvestmentPortfolio", action = "AssetsDetails", portfolioId = vm.PortfolioId });
            }

            var investmentPortfolio = await _investmentPortfolioService.GetById(vm.PortfolioId, userId);

            if (investmentPortfolio == null)
            {
                return RedirectToRoute(new { controller = "InvestmentPortfolio", action = "AssetsDetails", portfolioId = vm.PortfolioId });
            }

            await _investmentAssetsService.DeleteAsync(vm.Id);
            return RedirectToRoute(new { controller = "InvestmentPortfolio", action = "AssetsDetails", portfolioId = vm.PortfolioId });
        }
    }
}

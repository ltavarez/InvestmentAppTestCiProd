using AutoMapper;
using InvestmentApp.Core.Application.Dtos.InvestmentPortfolio;
using InvestmentApp.Core.Application.Interfaces;
using InvestmentApp.Core.Application.ViewModels.Asset;
using InvestmentApp.Core.Application.ViewModels.InvestmentPortfolio;
using InvestmentApp.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ItlaInvestmentApp.Controllers
{
    [Authorize]
    public class InvestmentPortfolioController : Controller
    {
        private readonly IInvestmentPortfolioService _investmentPortfolioService;
        private readonly IAssetService _assetService;
        private readonly IAssetTypeService _assetTypeService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IAccountServiceForWebApp _accountServiceForWebApp;
        public InvestmentPortfolioController(IInvestmentPortfolioService investmentPortfolioService, IAssetService assetService, IAssetTypeService assetTypeService, IMapper mapper, UserManager<AppUser> userManager, IAccountServiceForWebApp accountServiceForWebApp)
        {
            _investmentPortfolioService = investmentPortfolioService;
            _assetService = assetService;
            _assetTypeService = assetTypeService;
            _mapper = mapper;
            _userManager = userManager;
            _accountServiceForWebApp = accountServiceForWebApp;
        }
        public async Task<IActionResult> Index()
        {
            AppUser? userSession = await _userManager.GetUserAsync(User);
            if (userSession == null)
            {
                return RedirectToRoute(new { controller = "Login", action = "Index" });
            }
            var user = await _accountServiceForWebApp.GetUserByUserName(userSession.UserName ?? "");

            var dtos = await _investmentPortfolioService.GetAllWithIncludeByUser(userSession.Id);

            var listEntityVms = _mapper.Map<List<InvestmentPortfolioViewModel>>(dtos);

            ViewBag.NameUser = user?.Name ?? "";
            ViewBag.LastNameUser = user?.LastName ?? "";
            return View(listEntityVms);
        }
        public IActionResult Create()
        {
            return View("Save", new SaveInvestmentPortfolioViewModel() { Name = "" });
        }

        [HttpPost]
        public async Task<IActionResult> Create(SaveInvestmentPortfolioViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View("Save", vm);
            }

            InvestmentPortfolioDto dto = _mapper.Map<InvestmentPortfolioDto>(vm);
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            dto.UserId = userId;      

            await _investmentPortfolioService.AddAsync(dto);
            return RedirectToRoute(new { controller = "InvestmentPortfolio", action = "Index" });
        }

        public async Task<IActionResult> AssetsDetails(int portfolioId, string? assetName = null, int? assetTypeId = null, int? assetOrderBy = null)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToRoute(new { controller = "InvestmentPortfolio", action = "Index" });
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

            var portfolioDto = await _investmentPortfolioService.GetById(portfolioId, userId);

            if (portfolioDto == null)
            {
                return RedirectToRoute(new { controller = "InvestmentPortfolio", action = "Index" });
            }

            InvestmentPortfolioViewModel portfolioVm = _mapper.Map<InvestmentPortfolioViewModel>(portfolioDto);

            var dtos = await _assetService.GetAllAssetsByPortfolioId(portfolioId, assetName, assetTypeId, assetOrderBy);

            var listEntityVms = _mapper.Map<List<AssetForPortfolioViewModel>>(dtos);

            ViewBag.Portfolio = portfolioVm;
            ViewBag.AssetTypes = await _assetTypeService.GetAll();

            return View("Details", listEntityVms);
        }
        public async Task<IActionResult> Edit(int id)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToRoute(new { controller = "InvestmentPortfolio", action = "Index" });
            }

            ViewBag.EditMode = true;
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var dto = await _investmentPortfolioService.GetById(id, userId);

            if (dto == null)
            {
                return RedirectToRoute(new { controller = "InvestmentPortfolio", action = "Index" });
            }

            SaveInvestmentPortfolioViewModel vm = _mapper.Map<SaveInvestmentPortfolioViewModel>(dto);

            return View("Save", vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SaveInvestmentPortfolioViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.EditMode = true;
                return View("Save", vm);
            }

            InvestmentPortfolioDto dto = _mapper.Map<InvestmentPortfolioDto>(vm);
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            dto.UserId = userId;

            var investmentDto = await _investmentPortfolioService.GetById(vm.Id, userId);

            if (investmentDto == null)
            {
                ViewBag.EditMode = true;
                return View("Save", vm);
            }

            await _investmentPortfolioService.UpdateAsync(dto, dto.Id);
            return RedirectToRoute(new { controller = "InvestmentPortfolio", action = "Index" });
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToRoute(new { controller = "InvestmentPortfolio", action = "Index" });
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var dto = await _investmentPortfolioService.GetById(id, userId);
            if (dto == null)
            {
                return RedirectToRoute(new { controller = "InvestmentPortfolio", action = "Index" });
            }
            DeleteInvestmentPortfolioViewModel vm = _mapper.Map<DeleteInvestmentPortfolioViewModel>(dto);
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(DeleteInvestmentPortfolioViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var dto = await _investmentPortfolioService.GetById(vm.Id, userId);

            if (dto == null)
            {
                return View(vm);
            }

            await _investmentPortfolioService.DeleteAsync(vm.Id);
            return RedirectToRoute(new { controller = "InvestmentPortfolio", action = "Index" });
        }
    }
}

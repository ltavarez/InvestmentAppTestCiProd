using AutoMapper;
using InvestmentApp.Core.Application.Dtos.Asset;
using InvestmentApp.Core.Application.Interfaces;
using InvestmentApp.Core.Application.ViewModels.Asset;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ItlaInvestmentApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AssetController : Controller
    {
        private readonly IAssetService _assetService;
        private readonly IAssetTypeService _assetTypeService;
        private readonly IMapper _mapper;

        public AssetController(IAssetService assetService, IAssetTypeService assetTypeService, IMapper mapper)
        {
            _assetService = assetService;
            _assetTypeService = assetTypeService;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {
            var dtos = await _assetService.GetAllWithInclude();

            var listEntityVms = _mapper.Map<List<AssetViewModel>>(dtos);

            return View(listEntityVms);
        }
        public async Task<IActionResult> Create()
        {
            ViewBag.AssetTypes = await _assetTypeService.GetAll();
            return View("Save", new SaveAssetViewModel() { Name = "", Symbol = "", AssetTypeId = null });
        }
        [HttpPost]
        public async Task<IActionResult> Create(SaveAssetViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.AssetTypes = await _assetTypeService.GetAll();
                return View("Save", vm);
            }

            AssetDto dto = _mapper.Map<AssetDto>(vm);

            await _assetService.AddAsync(dto);
            return RedirectToRoute(new { controller = "Asset", action = "Index" });
        }
        public async Task<IActionResult> Edit(int id)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToRoute(new { controller = "Asset", action = "Index" });
            }

            ViewBag.EditMode = true;
            ViewBag.AssetTypes = await _assetTypeService.GetAll();
            var dto = await _assetService.GetById(id);

            if (dto == null)
            {
                return RedirectToRoute(new { controller = "Asset", action = "Index" });
            }

            SaveAssetViewModel vm = _mapper.Map<SaveAssetViewModel>(dto);
            return View("Save", vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SaveAssetViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.EditMode = true;
                ViewBag.AssetTypes = await _assetTypeService.GetAll();
                return View("Save", vm);
            }

            AssetDto dto = _mapper.Map<AssetDto>(vm);
            await _assetService.UpdateAsync(dto, dto.Id);
            return RedirectToRoute(new { controller = "Asset", action = "Index" });
        }

        public async Task<IActionResult> Delete(int id)
        {        
            if (!ModelState.IsValid)
            {
                return RedirectToRoute(new { controller = "Asset", action = "Index" });
            }
            var dto = await _assetService.GetById(id);
            if (dto == null)
            {
                return RedirectToRoute(new { controller = "Asset", action = "Index" });
            }
            DeleteAssetViewModel vm = _mapper.Map<DeleteAssetViewModel>(dto);
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(DeleteAssetViewModel vm)
        {  
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            await _assetService.DeleteAsync(vm.Id);
            return RedirectToRoute(new { controller = "Asset", action = "Index" });
        }

    }
}

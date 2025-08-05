using AutoMapper;
using InvestmentApp.Core.Application.Dtos.AssetType;
using InvestmentApp.Core.Application.Interfaces;
using InvestmentApp.Core.Application.ViewModels.AssetType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ItlaInvestmentApp.Controllers
{

    [Authorize(Roles = "Admin")]
    public class AssetTypeController : Controller
    {
        private readonly IAssetTypeService _assetTypeService;
        private readonly IMapper _mapper;
        public AssetTypeController(IAssetTypeService assetTypeService, IMapper mapper)
        {
            _assetTypeService = assetTypeService;        
            _mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {
            var dtos = await _assetTypeService.GetAllWithInclude();

            var listEntityVms = _mapper.Map<List<AssetTypeViewModel>>(dtos);

            return View(listEntityVms);
        }
        public IActionResult Create()
        {  
            return View("Save", new SaveAssetTypeViewModel() { Name = "" });
        }

        [HttpPost]
        public async Task<IActionResult> Create(SaveAssetTypeViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View("Save", vm);
            }

            AssetTypeDto dto = _mapper.Map<AssetTypeDto>(vm);
            await _assetTypeService.AddAsync(dto);
            return RedirectToRoute(new { controller = "AssetType", action = "Index" });
        }
        public async Task<IActionResult> Edit(int id)
        {   
            if (!ModelState.IsValid)
            {
                return RedirectToRoute(new { controller = "AssetType", action = "Index" });
            }

            ViewBag.EditMode = true;
            var dto = await _assetTypeService.GetById(id);
            if (dto == null)
            {
                return RedirectToRoute(new { controller = "AssetType", action = "Index" });
            }
            SaveAssetTypeViewModel vm = _mapper.Map<SaveAssetTypeViewModel>(dto);
            return View("Save", vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SaveAssetTypeViewModel vm)
        {  
            if (!ModelState.IsValid)
            {
                ViewBag.EditMode = true;
                return View("Save", vm);
            }

            AssetTypeDto dto = _mapper.Map<AssetTypeDto>(vm);
            await _assetTypeService.UpdateAsync(dto, dto.Id);
            return RedirectToRoute(new { controller = "AssetType", action = "Index" });
        }
        public async Task<IActionResult> Delete(int id)
        { 
            if (!ModelState.IsValid)
            {
                return RedirectToRoute(new { controller = "AssetType", action = "Index" });
            }

            var dto = await _assetTypeService.GetById(id);
            if (dto == null)
            {
                return RedirectToRoute(new { controller = "AssetType", action = "Index" });
            }
            DeleteAssetTypeViewModel vm = _mapper.Map<DeleteAssetTypeViewModel>(dto);
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(DeleteAssetTypeViewModel vm)
        {   
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            await _assetTypeService.DeleteAsync(vm.Id);
            return RedirectToRoute(new { controller = "AssetType", action = "Index" });
        }

    }
}

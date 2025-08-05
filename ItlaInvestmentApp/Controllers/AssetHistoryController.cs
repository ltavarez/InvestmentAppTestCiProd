using InvestmentApp.Core.Application.Dtos.AssetHistory;
using InvestmentApp.Core.Application.Interfaces;
using InvestmentApp.Core.Application.ViewModels.AssetHistory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ItlaInvestmentApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AssetHistoryController : Controller
    {
        private readonly IAssetHistoryService _assetHistoryService;   
        public AssetHistoryController(IAssetHistoryService assetHistoryService)
        {
            _assetHistoryService = assetHistoryService;      
        }
        public IActionResult Create(int assetId)
        {     

            if (!ModelState.IsValid)
            {
                return RedirectToRoute(new { controller = "Asset", action = "Index" });
            }

            return View("Save", new SaveAssetHistoryViewModel() { AssetId = assetId, Id = 0, Value = 0,HistoryValueDate = DateTime.UtcNow });
        }

        [HttpPost]
        public async Task<IActionResult> Create(SaveAssetHistoryViewModel vm)
        {  
            if (!ModelState.IsValid)
            {
                return View("Save", vm);
            }

            AssetHistoryDto dto = new()
            {
                Id = 0,
                AssetId = vm.AssetId,
                Value = vm.Value,
                HistoryValueDate = vm.HistoryValueDate
            };

            await _assetHistoryService.AddAsync(dto);
            return RedirectToRoute(new { controller = "Asset", action = "Index" });
        }
        public async Task<IActionResult> Edit(int id)
        {   
            if (!ModelState.IsValid)
            {
                return RedirectToRoute(new { controller = "Asset", action = "Index" });
            }

            ViewBag.EditMode = true;
            var dto = await _assetHistoryService.GetById(id);

            if (dto == null)
            {
                return RedirectToRoute(new { controller = "Asset", action = "Index" });
            }

            SaveAssetHistoryViewModel vm = new()
            {
                Id = dto.Id,
                AssetId = dto.AssetId,
                Value = dto.Value,
                HistoryValueDate = dto.HistoryValueDate,
            };

            return View("Save", vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SaveAssetHistoryViewModel vm)
        {  
            if (!ModelState.IsValid)
            {
                ViewBag.EditMode = true;
                ViewBag.AssetTypes = await _assetHistoryService.GetAll();
                return View("Save", vm);
            }

            AssetHistoryDto dto = new()
            {
                Id = vm.Id,
                AssetId = vm.AssetId,
                Value = vm.Value,
                HistoryValueDate = vm.HistoryValueDate,                
            };
            await _assetHistoryService.UpdateAsync(dto, dto.Id);
            return RedirectToRoute(new { controller = "Asset", action = "Index" });
        }
        public async Task<IActionResult> Delete(int id)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToRoute(new { controller = "Asset", action = "Index" });
            }

            var dto = await _assetHistoryService.GetById(id);
            if (dto == null)
            {
                return RedirectToRoute(new { controller = "Asset", action = "Index" });
            }
            DeleteAssetHistoryViewModel vm = new() { Id = dto.Id, HistoryValueDate = dto.HistoryValueDate.ToShortDateString() };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(DeleteAssetHistoryViewModel vm)
        {  
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            await _assetHistoryService.DeleteAsync(vm.Id);
            return RedirectToRoute(new { controller = "Asset", action = "Index" });
        }
    }
}

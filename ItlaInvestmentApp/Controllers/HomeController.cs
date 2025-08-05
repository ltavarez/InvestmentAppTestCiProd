using InvestmentApp.Core.Application.Interfaces;
using InvestmentApp.Core.Domain.Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ItlaInvestmentApp.Controllers
{
    [Authorize(Roles ="Admin")]
    public class HomeController : Controller
    {         
        public IActionResult Index()
        {    
            return View();
        }      
    }
}

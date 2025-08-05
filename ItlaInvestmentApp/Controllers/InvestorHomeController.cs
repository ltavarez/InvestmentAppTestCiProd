using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ItlaInvestmentApp.Controllers
{
    [Authorize(Roles = "Investor")]
    public class InvestorHomeController : Controller
    {   
        public IActionResult Index()
        {
            return View();
        }      
    }
}

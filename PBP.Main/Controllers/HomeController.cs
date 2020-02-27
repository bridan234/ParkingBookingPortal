using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PBP.DataAccess;
using PBP.Main.Models;
using PBP.Pocos;

namespace PBP.Main.Controllers
{
    public class HomeController : Controller
    {
        private DBModel db = new DBModel();
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            IDataRepository<CalendarPoco> repo = new DataRepository<CalendarPoco>();
            IList<DateTime> UnAvailableDates = repo.GetList(days => days.AvailableSlots <= 0).Select(d => d.Date).ToList();
            
            return View(UnAvailableDates);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

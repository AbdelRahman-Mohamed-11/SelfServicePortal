using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SelfServicePortal.Core.DTOs;
using SelfServicePortal.Core.Interfaces;
using SelfServicePortal.Web.Models;

namespace SelfServicePortal.Web.Controllers
{
    public class HomeController(
        ILogger<HomeController> logger,
        IIncidentService incidentService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var filter = new IncidentFilterDto
            {
                PageNumber = 1,
                PageSize = 10
            };

            var (items, totalCount) = await incidentService.GetFilteredIncidentsAsync(filter);

            var model = new IncidentListViewModel
            {
                Incidents = new PaginatedList<IncidentDto>(items, totalCount, filter.PageNumber, filter.PageSize)
            };

            return View(model);
        }
    }
}

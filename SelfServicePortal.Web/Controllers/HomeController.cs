using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SelfServicePortal.Core.DTOs;
using SelfServicePortal.Core.Entities.Identity.Enums;
using SelfServicePortal.Core.Interfaces;
using SelfServicePortal.Web.Models;

namespace SelfServicePortal.Web.Controllers
{
    [Authorize]
    public class HomeController(
        IIncidentService incidentService , 
        ILogger<HomeController> logger) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            logger.LogInformation("HomeController.Index called");

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Forbid();  

            var currentUserId = Guid.Parse(userIdClaim);
            var isAdmin = User.IsInRole(Role.Admin.ToString());
            var isERP = User.IsInRole(Role.ERP.ToString());

            logger.LogInformation(
                "User {UserId} roles - Admin: {IsAdmin}, ERP: {IsERP}",
                currentUserId, isAdmin, isERP);

            var filter = new IncidentFilterDto
            {
                PageNumber = 1,
                PageSize = 10
            };


            logger.LogInformation(
                "Fetching incidents with PageNumber={PageNumber}, PageSize={PageSize}",
                filter.PageNumber, filter.PageSize);

            var (items, totalCount) = await incidentService
                .GetFilteredIncidentsAsync(
                    filter,
                    currentUserId,
                    isAdmin,
                    isERP);

            var model = new IncidentListViewModel
            {
                Incidents = new PaginatedList<IncidentDto>(
                    items,
                    totalCount,
                    filter.PageNumber,
                    filter.PageSize)
            };

            return View(model);
        }
    }
}

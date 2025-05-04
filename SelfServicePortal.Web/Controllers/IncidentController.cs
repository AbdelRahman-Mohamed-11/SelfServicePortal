using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SelfServicePortal.Core.Entities;
using SelfServicePortal.Core.Entities.Identity.Enums;
using SelfServicePortal.Web.Models;
using System.Security.Claims;

namespace SelfServicePortal.Web.Controllers
{
    [Authorize]
    public class IncidentController(IIncidentService incidentService, IWebHostEnvironment webHostEnvironment) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new CreateIncidentViewModel
            {
                RecurringIncidents = (await incidentService.GetRecurringIncidentsAsync())
                    .Select(i => new SelectListItem
                    {
                        Value = i.Id.ToString(),
                        Text = $"{i.CallRef} - {i.Subject}"
                    }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateIncidentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.RecurringIncidents = (await incidentService.GetRecurringIncidentsAsync())
                    .Select(i => new SelectListItem
                    {
                        Value = i.Id.ToString(),
                        Text = $"{i.CallRef} - {i.Subject}"
                    }).ToList();

                return View(model);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Forbid();

            var loggedById = Guid.Parse(userIdClaim);

            var incident = new Incident(
                loggedById,
                Enum.Parse<CallType>(model.CallType),
                Enum.Parse<Module>(model.Module),
                model.UrlFormName,
                model.IsRecurring,
                model.IsRecurring ? model.RecurringCallId : null,
                Enum.Parse<Priority>(model.Priority),
                model.Subject,
                model.Description,
                callRef: "",
                model.Suggestion
            );

            incident.SetCallRef();

            if (model.Attachments != null && model.Attachments.Length > 0)
            {
                var uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);

                foreach (var file in model.Attachments)
                {
                    if (file.Length > 0)
                    {
                        var fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
                        var filePath = Path.Combine(uploadsFolder, fileName);

                        using var stream = new FileStream(filePath, FileMode.Create);
                        await file.CopyToAsync(stream);

                        var attachment = new IncidentAttachment(incident.Id, file.FileName, $"/uploads/{fileName}", incident.LoggedById);
                        
                    }
                }
            }

            await incidentService.CreateIncidentAsync(incident);
            return RedirectToAction(nameof(Index));
        }
    }
}

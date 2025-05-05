using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SelfServicePortal.Core.DTOs;
using SelfServicePortal.Core.Entities;
using SelfServicePortal.Core.Entities.Identity.Enums;
using SelfServicePortal.Core.Interfaces;
using SelfServicePortal.Web.Extensions;
using SelfServicePortal.Web.Models;
using System.Security.Claims;

namespace SelfServicePortal.Web.Controllers
{
    [Authorize]
    public class IncidentController : Controller
    {
        private readonly IIncidentService incidentService;
        private readonly IUserService userService;
        private readonly IWebHostEnvironment webHostEnvironment;

        public IncidentController(
            IIncidentService incidentService,
            IUserService userService,
            IWebHostEnvironment webHostEnvironment)
        {
            this.incidentService = incidentService;
            this.userService = userService;
            this.webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var incident = await incidentService.GetIncidentByIdAsync(id);
            if (incident == null)
                return NotFound();

            var model = new IncidentDetailsViewModel
            {
                Id = incident.Id,
                CallRef = incident.CallRef,
                CreatedDate = incident.CreatedDate,
                Subject = incident.Subject,
                Description = incident.Description,
                Suggestion = incident?.Suggestion ?? "",
                SupportStatus = incident!.SupportStatus,
                UserStatus = incident.UserStatus,
                AssignedTo = incident.AssignedTo?.UserName ?? "-",
                DeliveryDate = incident.DeliveryDate,
                ReportedBy = incident.LoggedBy.UserName!,
                ClosedDate = incident.ClosedDate,
                StatusUpdatedDate = incident.StatusUpdatedDate,
                Module = incident.Module.ToString(),
                Priority = incident.Priority.ToString(),
                CallType = incident.CallType.ToString(),
                Attachments = incident.Attachments.Select(a => new IncidentAttachmentViewModel
                {
                    Id = a.Id,
                    FileName = a.FileName,
                    FilePath = a.FilePath
                }).ToList()
            };

            return View(model);
        }

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
            await incidentService.CreateIncidentAsync(incident);

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

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Index(IncidentFilterViewModel filter)
        {
            var model = new IncidentListViewModel
            {
                Filter = filter,
                Users = await userService.GetAllUsersAsSelectListAsync(),
                CallTypes = Enum.GetValues<CallType>()
                    .Select(e => new SelectListItem
                    {
                        Value = e.ToString(),
                        Text = e.ToString()
                    }).ToList(),
                Modules = Enum.GetValues<Module>()
                    .Select(e => new SelectListItem
                    {
                        Value = e.ToString(),
                        Text = e.ToString()
                    }).ToList(),
                Priorities = Enum.GetValues<Priority>()
                    .Select(e => new SelectListItem
                    {
                        Value = e.ToString(),
                        Text = e.ToString()
                    }).ToList(),
                SupportStatuses = Enum.GetValues<SupportStatus>()
                    .Select(e => new SelectListItem
                    {
                        Value = e.ToString(),
                        Text = e.ToString()
                    }).ToList(),
                UserStatuses = Enum.GetValues<UserStatus>()
                    .Select(e => new SelectListItem
                    {
                        Value = e.ToString(),
                        Text = e.ToString()
                    }).ToList()
            };

            var (items, totalCount) = await incidentService.GetFilteredIncidentsAsync(filter.ToDto());
            model.Incidents = new PaginatedList<IncidentDto>(items, totalCount, filter.PageNumber, filter.PageSize);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var incident = await incidentService.GetIncidentByIdAsync(id);
            if (incident == null)
                return NotFound();

            var model = new UpdateIncidentViewModel
            {
                Id = incident.Id,
                CallRef = incident.CallRef,
                CreatedDate = incident.CreatedDate,
                Subject = incident.Subject,
                Description = incident.Description,
                Suggestion = incident.Suggestion,
                SupportStatus = incident.SupportStatus,
                UserStatus = incident.UserStatus,
                AssignedToId = incident.AssignedToId,
                DeliveryDate = incident.DeliveryDate,
                Users = await userService.GetAllUsersAsSelectListAsync(),
                SupportStatuses = Enum.GetValues<SupportStatus>()
                    .Select(e => new SelectListItem
                    {
                        Value = e.ToString(),
                        Text = e.ToString()
                    }).ToList(),
                UserStatuses = Enum.GetValues<UserStatus>()
                    .Select(e => new SelectListItem
                    {
                        Value = e.ToString(),
                        Text = e.ToString()
                    }).ToList(),
                Attachments = incident.Attachments.Select(a => new IncidentAttachmentViewModel
                {
                    Id = a.Id,
                    FileName = a.FileName,
                    FilePath = a.FilePath
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateIncidentViewModel model, Guid[] attachmentsToRemove)
        {
            if (!ModelState.IsValid)
            {
                model.Users = await userService.GetAllUsersAsSelectListAsync();
                model.SupportStatuses = Enum.GetValues<SupportStatus>()
                    .Select(e => new SelectListItem
                    {
                        Value = e.ToString(),
                        Text = e.ToString()
                    }).ToList();
                model.UserStatuses = Enum.GetValues<UserStatus>()
                    .Select(e => new SelectListItem
                    {
                        Value = e.ToString(),
                        Text = e.ToString()
                    }).ToList();
                return View(model);
            }

            var incident = await incidentService.GetIncidentByIdAsync(model.Id);
            if (incident == null)
                return NotFound();

            incident.SetSuggestion(model.Suggestion);
            incident.SetUserStatus(model.UserStatus);
            incident.SetSupportStatus(model.SupportStatus);
            if (User.IsInRole("Admin"))
            {
                incident.SetAssignedTo(model.AssignedToId);
            }
            incident.SetDeliveryDate(model.DeliveryDate ?? DateTime.UtcNow);
            incident.SetStatusUpdatedDate(DateTime.UtcNow);

            if (model.SupportStatus == SupportStatus.Delivered)
            {
                incident.SetClosedDate(DateTime.UtcNow);
            }

            // Handle attachment removals
            if (attachmentsToRemove != null && attachmentsToRemove.Length > 0)
            {
                foreach (var attachmentId in attachmentsToRemove)
                {
                    await incidentService.RemoveAttachmentAsync(attachmentId);
                }
            }

            if (model.NewAttachments != null && model.NewAttachments.Length > 0)
            {
                var uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);

                foreach (var file in model.NewAttachments)
                {
                    if (file.Length > 0)
                    {
                        var fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
                        var filePath = Path.Combine(uploadsFolder, fileName);

                        using var stream = new FileStream(filePath, FileMode.Create);
                        await file.CopyToAsync(stream);

                        var attachment = new IncidentAttachment(incident.Id, file.FileName, $"/uploads/{fileName}", incident.LoggedById);
                        await incidentService.AddAttachmentAsync(attachment);
                    }
                }
            }

            await incidentService.UpdateIncidentAsync(incident);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(Guid id, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return BadRequest();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            await incidentService.AddCommentAsync(id, text, Guid.Parse(userId));
            var comments = await incidentService.GetIncidentCommentsAsync(id);
            var viewModel = comments.Select(c => new IncidentCommentViewModel
            {
                Id = c.Id,
                Text = c.Text,
                CreatorName = c.Creator.UserName!,
                CreatedDate = c.CreatedAt,
                CanDelete = c.CreatorId.ToString() == userId || User.IsInRole(nameof(Role.Admin))
            }).ToList();

            return PartialView("_CommentsList", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(Guid id, Guid commentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            try
            {
                await incidentService.RemoveCommentAsync(commentId, Guid.Parse(userId));
                return Json(new { success = true });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
        public async Task<IActionResult> GetComments(Guid id)
        {
            var comments = await incidentService.GetIncidentCommentsAsync(id);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var viewModel = comments.Select(c => new IncidentCommentViewModel
            {
                Id = c.Id,
                Text = c.Text,
                CreatorName = c.Creator.UserName!,
                CreatedDate = c.CreatedAt,
                CanDelete = c.CreatorId.ToString() == userId || User.IsInRole(nameof(Role.Admin))
            }).ToList();

            return PartialView("Components/Comments/Default", viewModel);
        }
    }
}

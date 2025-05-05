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
    public class IncidentController(
            IIncidentService incidentService,
            IUserService userService,
            IAuthorizationService auth,
            IWebHostEnvironment webHostEnvironment,
            ILogger<IncidentController> logger
        ) : Controller
    {
        private readonly IIncidentService _incidentService = incidentService;
        private readonly IUserService _userService = userService;
        private readonly IAuthorizationService _auth = auth;
        private readonly IWebHostEnvironment _env = webHostEnvironment;
        private readonly ILogger<IncidentController> _logger = logger;

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            _logger.LogInformation("GET Details called for Incident {IncidentId}", id);

            var incident = await _incidentService.GetIncidentByIdAsync(id);
            if (incident == null)
            {
                _logger.LogWarning("Incident {IncidentId} not found", id);
                return NotFound();
            }

            _logger.LogInformation("Loaded Incident {IncidentId}", id);
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

            _logger.LogInformation("Returning Details view for Incident {IncidentId}", id);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("GET Create Incident page requested");

            var recurring = await _incidentService.GetRecurringIncidentsAsync();
            var model = new CreateIncidentViewModel
            {
                RecurringIncidents = recurring
                    .Select(i => new SelectListItem
                    {
                        Value = i.Id.ToString(),
                        Text = $"{i.CallRef} - {i.Subject}"
                    })
                    .ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateIncidentViewModel model)
        {
            _logger.LogInformation("POST Create Incident attempt: {@Model}", model);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Create Incident model invalid");
                model.RecurringIncidents = (await _incidentService.GetRecurringIncidentsAsync())
                    .Select(i => new SelectListItem
                    {
                        Value = i.Id.ToString(),
                        Text = $"{i.CallRef} - {i.Subject}"
                    })
                    .ToList();
                return View(model);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
            {
                _logger.LogWarning("Create Incident attempted without authenticated user");
                return Forbid();
            }

            var loggedById = Guid.Parse(userIdClaim);
            _logger.LogInformation("Authenticated as User {UserId}", loggedById);

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
            await _incidentService.CreateIncidentAsync(incident);
            _logger.LogInformation("Created Incident {IncidentId}", incident.Id);

            if (model.Attachments != null && model.Attachments.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);

                foreach (var file in model.Attachments)
                {
                    if (file.Length > 0)
                    {
                        var fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
                        var filePath = Path.Combine(uploadsFolder, fileName);

                        using var stream = new FileStream(filePath, FileMode.Create);
                        await file.CopyToAsync(stream);

                        _logger.LogInformation(
                            "Saved attachment {FileName} for Incident {IncidentId}",
                            fileName, incident.Id);
                    }
                }

                TempData["SuccessMessage"] = "Attachments uploaded successfully!";
            }


            TempData["SuccessMessage"] = "Incident created successfully!";

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Index(IncidentFilterViewModel filter)
        {
            _logger.LogInformation("GET Index called with filter {@Filter}", filter);

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                _logger.LogWarning("Index attempted without authenticated user");
                return Forbid();
            }

            var currentUserId = Guid.Parse(userIdClaim);
            var isAdmin = User.IsInRole(Role.Admin.ToString());
            var isERP = User.IsInRole(Role.ERP.ToString());
            _logger.LogInformation(
                "User {UserId} roles - Admin: {IsAdmin}, ERP: {IsERP}",
                currentUserId, isAdmin, isERP);

            var model = new IncidentListViewModel
            {
                Filter = filter,
                Users = await _userService.GetAllUsersAsSelectListAsync(),
                CallTypes = Enum.GetValues<CallType>()
                    .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() })
                    .ToList(),
                Modules = Enum.GetValues<Module>()
                    .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() })
                    .ToList(),
                Priorities = Enum.GetValues<Priority>()
                    .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() })
                    .ToList(),
                SupportStatuses = Enum.GetValues<SupportStatus>()
                    .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() })
                    .ToList(),
                UserStatuses = Enum.GetValues<UserStatus>()
                    .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() })
                    .ToList()
            };

            var (items, totalCount) = await _incidentService.GetFilteredIncidentsAsync(
                filter.ToDto(),
                currentUserId,
                isAdmin,
                isERP);

            _logger.LogInformation(
                "Fetched {Count} incidents out of {TotalCount}",
                items.Count, totalCount);

            model.Incidents = new PaginatedList<IncidentDto>(
                items, totalCount, filter.PageNumber, filter.PageSize);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            _logger.LogInformation("GET Edit called for Incident {IncidentId}", id);

            var incident = await _incidentService.GetIncidentByIdAsync(id);
            if (incident == null)
            {
                _logger.LogWarning("Incident {IncidentId} not found for Edit", id);
                return NotFound();
            }

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
                Users = await _userService.GetAllUsersAsSelectListAsync(),
                SupportStatuses = Enum.GetValues<SupportStatus>()
                    .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() })
                    .ToList(),
                UserStatuses = Enum.GetValues<UserStatus>()
                    .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() })
                    .ToList(),
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
            _logger.LogInformation("POST Edit Incident {IncidentId}", model.Id);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Edit model invalid for Incident {IncidentId}", model.Id);
                model.Users = await _userService.GetAllUsersAsSelectListAsync();
                model.SupportStatuses = Enum.GetValues<SupportStatus>()
                    .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() })
                    .ToList();
                model.UserStatuses = Enum.GetValues<UserStatus>()
                    .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() })
                    .ToList();
                return View(model);
            }

            var incident = await _incidentService.GetIncidentByIdAsync(model.Id);
            if (incident == null)
            {
                _logger.LogWarning("Incident {IncidentId} not found on Edit POST", model.Id);
                return NotFound();
            }

            var isNormalUser = User.IsInRole(nameof(Role.User));
            if (isNormalUser)
            {
                incident.SetUserStatus(model.UserStatus);
            }
            else
            {
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
            }

            if (attachmentsToRemove != null && attachmentsToRemove.Length > 0)
            {
                foreach (var attachmentId in attachmentsToRemove)
                {
                    await _incidentService.RemoveAttachmentAsync(attachmentId);
                    _logger.LogInformation(
                        "Removed attachment {AttachmentId} from Incident {IncidentId}",
                        attachmentId, model.Id);
                }
            }

            if (model.NewAttachments != null && model.NewAttachments.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);

                foreach (var file in model.NewAttachments)
                {
                    if (file.Length > 0)
                    {
                        var fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
                        var filePath = Path.Combine(uploadsFolder, fileName);

                        using var stream = new FileStream(filePath, FileMode.Create);
                        await file.CopyToAsync(stream);
                        _logger.LogInformation(
                            "Added new attachment {FileName} to Incident {IncidentId}",
                            fileName, model.Id);

                        var attachment = new IncidentAttachment(
                            incident.Id,
                            file.FileName,
                            $"/uploads/{fileName}",
                            incident.LoggedById);
                        await _incidentService.AddAttachmentAsync(attachment);
                    }
                }

                TempData["SuccessMessage"] = "New attachments added successfully!";
            }

            await _incidentService.UpdateIncidentAsync(incident);
            _logger.LogInformation("Updated Incident {IncidentId}", model.Id);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(Guid id, string text)
        {
            _logger.LogInformation("POST AddComment called for Incident {IncidentId}", id);

            if (string.IsNullOrWhiteSpace(text))
            {
                _logger.LogWarning(
                    "AddComment received empty text for Incident {IncidentId}", id);
                return BadRequest();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                _logger.LogWarning("AddComment attempted without authenticated user");
                return Unauthorized();
            }

            await _incidentService.AddCommentAsync(id, text, Guid.Parse(userId));
            _logger.LogInformation(
                "Added comment by User {UserId} to Incident {IncidentId}", userId, id);

            var comments = await _incidentService.GetIncidentCommentsAsync(id);
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
            _logger.LogInformation(
                "POST DeleteComment {CommentId} on Incident {IncidentId}", commentId, id);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                _logger.LogWarning("DeleteComment attempted without authenticated user");
                return Unauthorized();
            }

            try
            {
                await _incidentService.RemoveCommentAsync(commentId, Guid.Parse(userId));
                _logger.LogInformation(
                    "Deleted comment {CommentId} by User {UserId}", commentId, userId);
                return Json(new { success = true });
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning(
                    "Unauthorized delete attempt for Comment {CommentId} by User {UserId}",
                    commentId, userId);
                return Forbid();
            }
        }

        public async Task<IActionResult> GetComments(Guid id)
        {
            _logger.LogInformation("GET GetComments called for Incident {IncidentId}", id);

            var comments = await _incidentService.GetIncidentCommentsAsync(id);
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

using Microsoft.AspNetCore.Mvc;
using SelfServicePortal.Core.Entities.Identity.Enums;
using SelfServicePortal.Core.Interfaces;
using System.Security.Claims;

public class CommentsViewComponent : ViewComponent
{
    private readonly IIncidentService incidentService;
    private readonly IHttpContextAccessor httpContextAccessor;

    public CommentsViewComponent(IIncidentService incidentService, IHttpContextAccessor httpContextAccessor)
    {
        this.incidentService = incidentService;
        this.httpContextAccessor = httpContextAccessor;
    }

    public async Task<IViewComponentResult> InvokeAsync(Guid incidentId, bool isReadOnly = false)
    {
        var comments = await incidentService.GetIncidentCommentsAsync(incidentId);
        var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        var viewModel = comments.Select(c => new IncidentCommentViewModel
        {
            Id = c.Id,
            Text = c.Text,
            CreatorName = c.Creator.UserName,
            CreatedDate = c.CreatedAt,
            CanDelete = c.CreatorId.ToString() == userId || User.IsInRole(nameof(Role.Admin))
        }).ToList();

        return View(isReadOnly ? "ReadOnly" : "Default", viewModel);
    }
}
using Microsoft.EntityFrameworkCore;
using SelfServicePortal.Core.DTOs;
using SelfServicePortal.Core.Entities;
using SelfServicePortal.Core.Interfaces;

namespace SelfServicePortal.Application.Services;

public class IncidentService(ITicketDbContext context , IUserService userService) : IIncidentService
{
    public async Task AddAttachmentAsync(IncidentAttachment attachment)
    {
        context.IncidentAttachments.Add(attachment);
        await context.SaveChangesAsync();
    }

    public async Task<Incident> CreateIncidentAsync(Incident incident)
    {
        context.Incidents.Add(incident);
        await context.SaveChangesAsync();
        return incident;
    }

    public async Task<IEnumerable<Incident>> GetRecurringIncidentsAsync()
    {
        return await context.Incidents
            .Where(i => !i.IsDeleted && i.IsRecurring)
            .OrderByDescending(i => i.CreatedDate)
            .ToListAsync();
    }

    public async Task<(List<IncidentDto> Items, int TotalCount)> GetFilteredIncidentsAsync(
        IncidentFilterDto filter)
    {
        var query = context.Incidents
            .Include(i => i.LoggedBy)
            .Include(i => i.AssignedTo)
            .Where(i => !i.IsDeleted);

        if (filter.CallType.HasValue)
            query = query.Where(i => i.CallType == filter.CallType);

        if (filter.Module.HasValue)
            query = query.Where(i => i.Module == filter.Module);

        if (filter.Priority.HasValue)
            query = query.Where(i => i.Priority == filter.Priority);

        if (filter.SupportStatus.HasValue)
            query = query.Where(i => i.SupportStatus == filter.SupportStatus);

        if (filter.UserStatus.HasValue)
            query = query.Where(i => i.UserStatus == filter.UserStatus);

        if (filter.AssignedToId.HasValue)
            query = query.Where(i => i.AssignedToId == filter.AssignedToId);

        if (filter.FromDate.HasValue)
            query = query.Where(i => i.CreatedDate >= filter.FromDate);

        if (filter.ToDate.HasValue)
            query = query.Where(i => i.CreatedDate <= filter.ToDate);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(i => i.CreatedDate)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(i => new IncidentDto
            {
                Id = i.Id,
                CallRef = i.CallRef,
                LogDate = i.CreatedDate,
                DeliveryDate = i.DeliveryDate,
                Description = i.Description,
                Subject = i.Subject,
                SupportStatus = i.SupportStatus.ToString(),
                UserStatus = i.UserStatus.ToString(),
                CallType = i.CallType.ToString(),
                Priority = i.Priority.ToString(),
                Module = i.Module.ToString(),
                ReportedBy = i.LoggedBy.UserName!,
                ClosedDate = i.ClosedDate,
                StatusUpdatedDate = i.StatusUpdatedDate,
                UrlOrFormName = i.UrlOrFormName,
                AssignedTo = i.AssignedTo != null ? i.AssignedTo.UserName : null
            })
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Incident?> GetIncidentByIdAsync(Guid id)
    {
        return await context.Incidents
            .Include(i => i.LoggedBy)
            .Include(i => i.AssignedTo)
            .Include(i => i.Attachments)
            .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
    }

    public async Task RemoveAttachmentAsync(Guid attachmentId)
    {
        var attachment = await context.IncidentAttachments.FindAsync(attachmentId);
        if (attachment != null)
        {
            context.IncidentAttachments.Remove(attachment);
            await context.SaveChangesAsync();

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", attachment.FilePath.TrimStart('/'));
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    public async Task UpdateIncidentAsync(Incident incident)
    {
        context.Incidents.Update(incident);
        await context.SaveChangesAsync();
    }
    public async Task<List<IncidentComment>> GetIncidentCommentsAsync(Guid incidentId)
    {
        return await context.IncidentComments
            .Include(c => c.Creator)
            .Where(c => c.IncidentId == incidentId && !c.IsDeleted)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task AddCommentAsync(Guid incidentId, string text, Guid creatorId)
    {
        var comment = new IncidentComment(incidentId, text, creatorId);
        context.IncidentComments.Add(comment);
        await context.SaveChangesAsync();
    }

    public async Task RemoveCommentAsync(Guid commentId, Guid userId)
    {
        var comment = await context.IncidentComments.FindAsync(commentId);
        if (comment == null) return;

        if (comment.CreatorId != userId && !await userService.IsAdminAsync(userId))
        {
            throw new UnauthorizedAccessException();
        }

        comment.IsDeleted = true;
        await context.SaveChangesAsync();
    }

}
using SelfServicePortal.Core.DTOs;
using SelfServicePortal.Core.Entities;

namespace SelfServicePortal.Core.Interfaces;

public interface IIncidentService
{
    Task<Incident> CreateIncidentAsync(Incident incident);
    Task<IEnumerable<Incident>> GetRecurringIncidentsAsync();
    Task AddAttachmentAsync(IncidentAttachment attachment);
    Task<(List<IncidentDto> Items, int TotalCount)> GetFilteredIncidentsAsync(
           IncidentFilterDto filter,
           Guid currentUserId,
           bool isAdmin,
           bool isERP);
    Task<Incident?> GetIncidentByIdAsync(Guid id);
    Task RemoveAttachmentAsync(Guid attachmentId);
    Task UpdateIncidentAsync(Incident incident);
    Task<List<IncidentComment>> GetIncidentCommentsAsync(Guid incidentId);
    Task AddCommentAsync(Guid incidentId, string text, Guid creatorId);
    Task RemoveCommentAsync(Guid commentId, Guid userId);
}
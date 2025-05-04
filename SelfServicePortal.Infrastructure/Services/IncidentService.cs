using Microsoft.EntityFrameworkCore;
using SelfServicePortal.Core.Entities;
using SelfServicePortal.Core.Interfaces;

namespace SelfServicePortal.Infrastructure.Services;

public class IncidentService(ITicketDbContext context) : IIncidentService
{
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
    public async Task AddAttachmentAsync(IncidentAttachment attachment)
    {
        context.IncidentAttachments.Add(attachment);
        await context.SaveChangesAsync();
    }
}
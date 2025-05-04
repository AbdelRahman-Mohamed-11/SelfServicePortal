using SelfServicePortal.Core.Entities;
using Microsoft.EntityFrameworkCore;
namespace SelfServicePortal.Core.Interfaces;

public interface ITicketDbContext
{
    DbSet<Incident> Incidents { get; }
    DbSet<IncidentComment> IncidentComments { get; }
    DbSet<IncidentAttachment> IncidentAttachments { get; }

    Task<int> SaveChangesAsync();
}

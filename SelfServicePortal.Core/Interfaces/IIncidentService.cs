using SelfServicePortal.Core.Entities;

public interface IIncidentService
{
    Task<Incident> CreateIncidentAsync(Incident incident);
    Task<IEnumerable<Incident>> GetRecurringIncidentsAsync();
    public async Task AddAttachmentAsync(IncidentAttachment attachment)

}
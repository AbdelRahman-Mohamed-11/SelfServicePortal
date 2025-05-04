using SelfServicePortal.Core.Entities;

public interface IIncidentService
{
    Task<Incident> CreateIncidentAsync(Incident incident);
    Task<IEnumerable<Incident>> GetRecurringIncidentsAsync();
    public Task AddAttachmentAsync(IncidentAttachment attachment);

}
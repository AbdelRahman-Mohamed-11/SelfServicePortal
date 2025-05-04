using SelfServicePortal.Core.Entities;

namespace SelfServicePortal.Application.Interfaces;

public interface IIncidentService
{
    Task<Incident> CreateIncidentAsync(Incident incident);
    Task<IEnumerable<Incident>> GetRecurringIncidentsAsync();
}
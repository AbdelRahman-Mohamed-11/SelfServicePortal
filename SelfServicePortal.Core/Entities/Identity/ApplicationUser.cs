using Microsoft.AspNetCore.Identity;

namespace SelfServicePortal.Core.Entities.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public virtual ICollection<Incident> LoggedIncidents { get; set; } = [];

    public virtual ICollection<Incident> AssignedIncidents { get; set; } = [];

    public virtual ICollection<IncidentComment> Comments { get; set; } = [];

    public virtual ICollection<IncidentAttachment> Attachments { get; set; } = [];
}

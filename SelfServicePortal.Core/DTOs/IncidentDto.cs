namespace SelfServicePortal.Core.DTOs;

public class IncidentDto
{
    public Guid Id { get; set; }
    public string CallRef { get; set; } = null!;
    public DateTime LogDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public string Description { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string SupportStatus { get; set; } = null!;
    public string UserStatus { get; set; } = null!;
    public string CallType { get; set; } = null!;
    public string Priority { get; set; } = null!;
    public string Module { get; set; } = null!;
    public string ReportedBy { get; set; } = null!;
    public DateTime? ClosedDate { get; set; }
    public DateTime StatusUpdatedDate { get; set; }
    public string UrlOrFormName { get; set; } = null!;
    public string? AssignedTo { get; set; }
}
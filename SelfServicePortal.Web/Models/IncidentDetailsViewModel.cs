using SelfServicePortal.Core.Entities.Identity.Enums;

namespace SelfServicePortal.Web.Models;

public class IncidentDetailsViewModel
{
    public Guid Id { get; set; }
    public string CallRef { get; set; } = "";
    public DateTime CreatedDate { get; set; }
    public string Subject { get; set; } = "";
    public string Description { get; set; } = "";
    public string Suggestion { get; set; } = "";
    public SupportStatus SupportStatus { get; set; }
    public UserStatus UserStatus { get; set; }
    public string AssignedTo { get; set; } = "";
    public DateTime? DeliveryDate { get; set; }
    public List<IncidentAttachmentViewModel> Attachments { get; set; } = [];
    public string ReportedBy { get; set; } = "";
    public DateTime? ClosedDate { get; set; }
    public DateTime StatusUpdatedDate { get; set; }
    public string Module { get; set; } = "";
    public string Priority { get; set; } = "";
    public string CallType { get; set; } = "";
}
using SelfServicePortal.Core.Entities.Identity.Enums;

namespace SelfServicePortal.Web.Models;

public class IncidentFilterViewModel
{
    public CallType? CallType { get; set; }
    public Module? Module { get; set; }
    public Priority? Priority { get; set; }
    public SupportStatus? SupportStatus { get; set; }
    public UserStatus? UserStatus { get; set; }
    public Guid? AssignedToId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}



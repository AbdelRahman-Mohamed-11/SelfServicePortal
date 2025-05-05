using SelfServicePortal.Core.Entities.Identity.Enums;

namespace SelfServicePortal.Core.DTOs;

public class IncidentFilterDto
{
    public CallType? CallType { get; set; }
    public Module? Module { get; set; }
    public Priority? Priority { get; set; }
    public SupportStatus? SupportStatus { get; set; }
    public UserStatus? UserStatus { get; set; }
    public Guid? AssignedToId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
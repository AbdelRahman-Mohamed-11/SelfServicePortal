using Microsoft.AspNetCore.Mvc.Rendering;
using SelfServicePortal.Core.DTOs;

namespace SelfServicePortal.Web.Models;

public class IncidentListViewModel
{
    public PaginatedList<IncidentDto> Incidents { get; set; } = null!;
    public IncidentFilterViewModel Filter { get; set; } = new();
    public List<SelectListItem> Users { get; set; } = [];
    public List<SelectListItem> CallTypes { get; set; } = [];
    public List<SelectListItem> Modules { get; set; } = [];
    public List<SelectListItem> Priorities { get; set; } = [];
    public List<SelectListItem> SupportStatuses { get; set; } = [];
    public List<SelectListItem> UserStatuses { get; set; } = [];
}

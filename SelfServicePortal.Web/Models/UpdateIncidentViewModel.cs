using Microsoft.AspNetCore.Mvc.Rendering;
using SelfServicePortal.Core.Entities.Identity.Enums;
using System.ComponentModel.DataAnnotations;

namespace SelfServicePortal.Web.Models;

public class UpdateIncidentViewModel
{
    public Guid Id { get; set; }

    [Display(Name = "Call Reference")]
    public string CallRef { get; set; } = null!;

    [Display(Name = "Created Date")]
    public DateTime CreatedDate { get; set; }

    [Display(Name = "Subject")]
    public string Subject { get; set; } = null!;

    [Display(Name = "Description")]
    public string Description { get; set; } = null!;

    [Display(Name = "Suggestion")]
    public string? Suggestion { get; set; }

    [Display(Name = "Support Status")]
    public SupportStatus SupportStatus { get; set; }

    [Display(Name = "User Status")]
    public UserStatus UserStatus { get; set; }

    [Display(Name = "Assigned To")]
    public Guid? AssignedToId { get; set; }

    [Display(Name = "Delivery Date")]
    [DataType(DataType.Date)]
    public DateTime? DeliveryDate { get; set; }

    public List<SelectListItem> Users { get; set; } = [];
    public List<SelectListItem> SupportStatuses { get; set; } = [];
    public List<SelectListItem> UserStatuses { get; set; } = [];

    public List<IncidentAttachmentViewModel> Attachments { get; set; } = [];
    public IFormFile[]? NewAttachments { get; set; }
}

public class IncidentAttachmentViewModel
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public bool IsSelected { get; set; }
}
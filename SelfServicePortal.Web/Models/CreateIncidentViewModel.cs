using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SelfServicePortal.Web.Models
{
    public class CreateIncidentViewModel
    {
        [Display(Name = "Call Type")]
        [Required(ErrorMessage = "Call type is required")]
        public string CallType { get; set; } = null!;

        [Display(Name = "Module")]
        [Required(ErrorMessage = "Module is required")]
        public string Module { get; set; } = null!;

        [Display(Name = "URL/Form Name")]
        [Required(ErrorMessage = "URL/Form name is required")]
        public string UrlFormName { get; set; } = null!;

        [Display(Name = "Recurring Call")]
        public bool IsRecurring { get; set; }

        [Display(Name = "Reference Call")]
        public Guid? RecurringCallId { get; set; }

        [Display(Name = "Priority")]
        [Required(ErrorMessage = "Priority is required")]
        public string Priority { get; set; } = null!;

        [Required(ErrorMessage = "Subject is required")]
        public string Subject { get; set; } = null!;

        [Display(Name = "Incident Description")]
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = null!;

        public string? Suggestion { get; set; }

        public IFormFile[]? Attachments { get; set; }

        public List<SelectListItem> RecurringIncidents { get; set; } = [];
    }
}
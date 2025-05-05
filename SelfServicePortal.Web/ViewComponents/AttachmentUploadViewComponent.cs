using Microsoft.AspNetCore.Mvc;

namespace SelfServicePortal.Web.ViewComponents;

public class AttachmentUploadViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(string? acceptedFileTypes = ".jpg,.jpeg,.png,.pdf,.doc,.docx")
    {
        return View(model: acceptedFileTypes);
    }
}
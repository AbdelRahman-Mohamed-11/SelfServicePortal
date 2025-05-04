using Microsoft.AspNetCore.Mvc;

public class AttachmentUploadViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(string? acceptedFileTypes = ".jpg,.jpeg,.png,.pdf,.doc,.docx")
    {
        return View(model: acceptedFileTypes);
    }
}
public class IncidentCommentViewModel
{
    public Guid Id { get; set; }
    public string Text { get; set; } = null!;
    public string CreatorName { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public bool CanDelete { get; set; }
}
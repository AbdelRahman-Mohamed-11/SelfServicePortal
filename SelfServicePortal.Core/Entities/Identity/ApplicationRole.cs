using Microsoft.AspNetCore.Identity;

namespace SelfServicePortal.Core.Entities.Identity
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public string Description { get; set; } = default!;

    }
}

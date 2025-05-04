using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SelfServicePortal.Core.Entities;
using SelfServicePortal.Core.Entities.Identity;
using SelfServicePortal.Core.Interfaces;
using SelfServicePortal.Infrastructure.Persistence.Configurations;

namespace SelfServicePortal.Infrastructure.Persistence
{
    public class TicketDbContext(DbContextOptions options)
                : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options), ITicketDbContext
    {
        public DbSet<Incident> Incidents { get; set; }
        public DbSet<IncidentComment> IncidentComments { get; set; }
        public DbSet<IncidentAttachment> IncidentAttachments { get; set; }

   
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationRoleConfiguration).Assembly);
        }
    }
}

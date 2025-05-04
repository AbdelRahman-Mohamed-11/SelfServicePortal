using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SelfServicePortal.Core.Entities;

namespace SelfServicePortal.Infrastructure.Persistence.Configurations
{
    public class IncidentCommentConfiguration : IEntityTypeConfiguration<IncidentComment>
    {
        public void Configure(EntityTypeBuilder<IncidentComment> builder)
        {
            builder.Property(ic => ic.Text)
                  .IsRequired()
                  .HasMaxLength(500);

            builder.HasQueryFilter(ic => !ic.IsDeleted);
        }
    }
}

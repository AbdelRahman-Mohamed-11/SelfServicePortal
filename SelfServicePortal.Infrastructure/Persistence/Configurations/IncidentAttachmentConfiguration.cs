using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SelfServicePortal.Core.Entities;

namespace SelfServicePortal.Infrastructure.Persistence.Configurations;

public class IncidentAttachmentConfiguration : IEntityTypeConfiguration<IncidentAttachment>
{
    public void Configure(EntityTypeBuilder<IncidentAttachment> builder)
    {
        builder.HasQueryFilter(i => !i.IsDeleted);
    }
}

using Gaia.IdP.DomainModel.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gaia.IdP.Data.EntityTypeConfigurations
{
    class ActivityLogEntityTypeConfiguration : IEntityTypeConfiguration<ActivityLog>
    {
        public void Configure(EntityTypeBuilder<ActivityLog> builder)
        {
            builder.ToTable("ActivityLogs");

            builder.HasKey(o => o.Id);

            builder.HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId);
        }
    }
}

using masterCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace masterInfrastructure.Data.Configuration
{
    class TenantConfig : IEntityTypeConfiguration<Tenant>
    {
        public void Configure(EntityTypeBuilder<Tenant> builder)
        {
            builder.ToTable("Tenants");

            builder.HasKey(k => k.Id);

            builder.Property(e => e.ConnectionString).IsRequired();

            builder.Property(e => e.Id).ValueGeneratedOnAdd();

            builder.Property(e => e.TenantName).IsRequired().HasMaxLength(250);
        }
    }
}

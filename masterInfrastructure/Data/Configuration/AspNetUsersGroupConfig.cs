using masterCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace masterInfrastructure.Data.Configuration
{
    class AspNetUsersGroupConfig : IEntityTypeConfiguration<AspNetUsersGroup>
    {
        public void Configure(EntityTypeBuilder<AspNetUsersGroup> builder)
        {

            builder.ToTable("AspNetUsersGroups");

            builder.HasKey(k => k.id);

            builder.Property(e => e.id).ValueGeneratedOnAdd();

            builder.Property(e => e.maximumDiscount).HasColumnType("numeric(18, 2)");

            builder.Property(e => e.name).HasMaxLength(250);

        }
    }
}

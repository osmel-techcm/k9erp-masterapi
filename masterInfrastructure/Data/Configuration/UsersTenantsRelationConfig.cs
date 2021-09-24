using masterCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace masterInfrastructure.Data.Configuration
{
    class UsersTenantsRelationConfig : IEntityTypeConfiguration<UsersTenantsRelation>
    {
        public void Configure(EntityTypeBuilder<UsersTenantsRelation> builder)
        {
            builder.ToTable("UsersTenantsRelations");

            builder.HasKey(k => k.Id);

            builder.Property(e => e.Id).ValueGeneratedOnAdd();

            builder.Property(e => e.UserId).HasMaxLength(450);
        }
    }
}

using masterCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace masterInfrastructure.Data.Configuration
{
    class CustomerConfig : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");

            builder.HasKey(k => k.Id);

            builder.Property(e => e.BillingCellForText).HasMaxLength(250);

            builder.Property(e => e.BillingContactTitle).HasMaxLength(250);

            builder.Property(e => e.BillingEmail).HasMaxLength(250);

            builder.Property(e => e.BillingLastName).HasMaxLength(250);

            builder.Property(e => e.BillingName).HasMaxLength(250);

            builder.Property(e => e.City).HasMaxLength(250);

            builder.Property(e => e.Country).HasMaxLength(250);

            builder.Property(e => e.CPL).HasColumnType("numeric(18, 2)");

            builder.Property(e => e.Email).HasMaxLength(250);

            builder.Property(e => e.Fax).HasMaxLength(250);

            builder.Property(e => e.FIEN).HasMaxLength(250);

            builder.Property(e => e.Id).ValueGeneratedOnAdd();

            builder.Property(e => e.LeadDate).HasColumnType("date");

            builder.Property(e => e.Name).IsRequired().HasMaxLength(250);

            builder.Property(e => e.Phone).HasMaxLength(250);

            builder.Property(e => e.State).HasMaxLength(250);

            builder.Property(e => e.TechnicalCellForText).HasMaxLength(250);

            builder.Property(e => e.TechnicalContactTitle).HasMaxLength(250);

            builder.Property(e => e.TechnicalEmail).HasMaxLength(250);

            builder.Property(e => e.TechnicalLastName).HasMaxLength(250);

            builder.Property(e => e.TechnicalName).HasMaxLength(250);

            builder.Property(e => e.Website).HasMaxLength(250);

            builder.Property(e => e.ZIP).HasMaxLength(50);
        }
    }
}

using Eticaret.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eticaret.Data.Configurations
{
    internal class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(50);           
            builder.Property(x => x.Image).HasMaxLength(250);
            builder.Property(x => x.ProductCode).HasMaxLength(50);
            builder.Property(x => x.Price).HasMaxLength(50).HasColumnType("Decimal(5,2)").HasMaxLength(50);
        }
    }
}

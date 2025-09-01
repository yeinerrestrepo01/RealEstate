using Microsoft.EntityFrameworkCore;
using RealEstate.Domain.Entities;

namespace RealEstate.Infrastructure
{
    public class RealEstateDbContext : DbContext
    {
        public RealEstateDbContext(DbContextOptions<RealEstateDbContext> options) : base(options) { }

        public DbSet<Property> Properties => Set<Property>();
        public DbSet<PropertyImage> PropertyImages => Set<PropertyImage>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Property>(e =>
            {
                e.ToTable("Properties");
                e.HasKey(p => p.Id);
                e.HasIndex(p => p.Code).IsUnique();
                e.Property(p => p.Code).HasMaxLength(32).IsRequired();
                e.Property(p => p.Title).HasMaxLength(160).IsRequired();
                e.Property(p => p.Description).HasMaxLength(4000);
                e.Property(p => p.Address).HasMaxLength(200).IsRequired();
                e.Property(p => p.City).HasMaxLength(100).IsRequired();
                e.Property(p => p.State).HasMaxLength(2).IsRequired(); // US state code
                e.Property(p => p.ZipCode).HasMaxLength(10).IsRequired();
                e.Property(p => p.Country).HasMaxLength(3).HasDefaultValue("USA");
                e.Property(p => p.Price).HasColumnType("decimal(18,2)");
                e.Property(p => p.Bathrooms).HasColumnType("decimal(5,2)");
                e.Property(p => p.LotSizeSqFt).HasColumnType("decimal(18,2)");
                e.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.Property(p => p.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

                e.HasMany(p => p.Images)
                 .WithOne(i => i.Property!)
                 .HasForeignKey(i => i.PropertyId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PropertyImage>(e =>
            {
                e.ToTable("PropertyImages");
                e.HasKey(i => i.Id);
                e.Property(i => i.Url).HasMaxLength(2048).IsRequired();
                e.HasIndex(i => new { i.PropertyId, i.IsCover });
            });
        }
    }
}

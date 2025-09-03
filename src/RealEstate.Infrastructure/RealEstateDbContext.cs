using Microsoft.EntityFrameworkCore;
using RealEstate.Domain.Entities;

namespace RealEstate.Infrastructure
{
    public class RealEstateDbContext : DbContext
    {
        public RealEstateDbContext(DbContextOptions<RealEstateDbContext> options) : base(options)
        {
        }

        public DbSet<Owner> Owners => Set<Owner>();
        public DbSet<Property> Properties => Set<Property>();
        public DbSet<PropertyImage> PropertyImages => Set<PropertyImage>();
        public DbSet<PropertyTrace> PropertyTraces => Set<PropertyTrace>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Owner>(e =>
            {
                e.ToTable("Owner");
                e.HasKey(x => x.IdOwner);
                e.Property(x => x.IdOwner).ValueGeneratedOnAdd();
                e.Property(x => x.Name).HasMaxLength(200).IsRequired();
                e.Property(x => x.Address).HasMaxLength(250).IsRequired();
                e.Property(x => x.Photo).HasMaxLength(2048);
            });

            modelBuilder.Entity<Property>(e =>
            {
                e.ToTable("Property");
                e.HasKey(x => x.IdProperty);
                e.Property(x => x.IdProperty).ValueGeneratedOnAdd();
                e.Property(x => x.Name).HasMaxLength(200).IsRequired();
                e.Property(x => x.Address).HasMaxLength(250).IsRequired();
                e.Property(x => x.Price).HasColumnType("decimal(18,2)");
                e.Property(x => x.CodeInternal).HasMaxLength(64).IsRequired();
                e.HasIndex(x => x.CodeInternal).IsUnique();
                e.Property(x => x.Year).IsRequired();

                e.HasOne(x => x.Owner)
                 .WithMany(o => o.Properties)
                 .HasForeignKey(x => x.IdOwner)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PropertyImage>(e =>
            {
                e.ToTable("PropertyImage");
                e.HasKey(x => x.IdPropertyImage);
                e.Property(x => x.IdPropertyImage).ValueGeneratedOnAdd();
                e.Property(x => x.Files).HasMaxLength(2048).IsRequired();
                e.Property(x => x.Enabled).HasDefaultValue(true);

                e.HasOne(x => x.Property)
                 .WithMany(p => p.Images)
                 .HasForeignKey(x => x.IdProperty)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasIndex(x => new { x.IdProperty, x.Enabled });
            });

            modelBuilder.Entity<PropertyTrace>(e =>
            {
                e.ToTable("PropertyTrace");
                e.HasKey(x => x.IdPropertyTrace);
                e.Property(x => x.IdPropertyTrace).ValueGeneratedOnAdd();
                e.Property(x => x.DateSale).IsRequired();
                e.Property(x => x.Name).HasMaxLength(200).IsRequired();
                e.Property(x => x.Value).HasColumnType("decimal(18,2)");
                e.Property(x => x.Tax).HasColumnType("decimal(18,2)");

                e.HasOne(x => x.Property)
                 .WithMany(p => p.Traces)
                 .HasForeignKey(x => x.IdProperty)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasIndex(x => new { x.IdProperty, x.DateSale });
            });
        }
    }
}
using CMS.Server.Models;
using Microsoft.EntityFrameworkCore;


namespace CMS.Server.EntityFrameworkCore
{
    public class AMSDbContext : DbContext
    {
        public AMSDbContext(DbContextOptions<AMSDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Cloth> Cloths { get; set; }
        public DbSet<Image> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User Entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id); // Primary Key

                entity.Property(u => u.Username)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(u => u.PasswordHash)
                      .IsRequired();

                entity.Property(u => u.Email)
                      .HasMaxLength(150);

                entity.Property(u => u.Role)
                      .IsRequired()
                      .HasMaxLength(50);

            });

            // Configure Cloth and Color relationship
            modelBuilder.Entity<Color>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ColorName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.AvailiableStock).IsRequired();

                // Explicitly configure the relationship
                entity.HasOne(c => c.Cloth) // Navigation property in Color
                      .WithMany(c => c.Colors) // Navigation property in Cloth
                      .HasForeignKey(c => c.ClothId) // Foreign key in Color
                      .OnDelete(DeleteBehavior.Cascade); // Optional: Configure delete behavior
            });

            // Configure Cloth entity
            modelBuilder.Entity<Cloth>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Price).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500);
            });

            // Configure Image and Color relationship
            modelBuilder.Entity<Image>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.URL).IsRequired().HasMaxLength(500);
                entity.Property(e => e.IsPrimary).IsRequired();

                // Explicitly configure the relationship
                entity.HasOne(i => i.Color) // Navigation property in Image
                      .WithMany(c => c.Images) // Navigation property in Color
                      .HasForeignKey(i => i.ColorId) // Foreign key in Image
                      .OnDelete(DeleteBehavior.Cascade); // Optional: Configure delete behavior
            });
        }
    }
}

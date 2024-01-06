using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using asp_net_core_web_app_authentication_authorisation.Models;

namespace asp_net_core_web_app_authentication_authorisation.Services
{
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser>
    {
        // Database entities
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Tour> Tours { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<HotelAvailability> HotelAvailabilities { get; set; }
        public DbSet<TourAvailability> TourAvailabilities { get; set; }

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Entity configurations
            modelBuilder.Entity<Tour>().HasKey(t => t.TourId);
            modelBuilder.Entity<Hotel>().HasKey(h => h.HotelId);
            modelBuilder.Entity<Package>().HasKey(tp => tp.PackageId);
            modelBuilder.Entity<HotelAvailability>().HasKey(tp => tp.HotelAvailabilityId);
            modelBuilder.Entity<TourAvailability>().HasKey(tp => tp.TourAvailabilityId);

            // Database relationships
            modelBuilder.Entity<Package>()
                .HasOne(p => p.Tour)
                .WithMany()
                .HasForeignKey(p => p.TourId);

            modelBuilder.Entity<Package>()
                .HasOne(p => p.Hotel)
                .WithMany()
                .HasForeignKey(p => p.HotelId);

            modelBuilder.Entity<HotelAvailability>()
               .HasOne(p => p.Hotel)
               .WithMany()
               .HasForeignKey(p => p.HotelId);

            modelBuilder.Entity<TourAvailability>()
               .HasOne(p => p.Tour)
               .WithMany()
               .HasForeignKey(p => p.TourId);

            base.OnModelCreating(modelBuilder);
        }
    }
}

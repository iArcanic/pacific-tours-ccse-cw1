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
        public DbSet<HotelBooking> HotelBookings { get; set; }
        public DbSet<TourBooking> TourBookings { get; set; }

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Entity configurations
            modelBuilder.Entity<Tour>().HasKey(t => t.TourId);
            modelBuilder.Entity<Hotel>().HasKey(h => h.HotelId);
            modelBuilder.Entity<Package>().HasKey(p => p.PackageId);
            modelBuilder.Entity<HotelAvailability>().HasKey(ha => ha.HotelAvailabilityId);
            modelBuilder.Entity<TourAvailability>().HasKey(ta => ta.TourAvailabilityId);
            modelBuilder.Entity<HotelBooking>().HasKey(hb => hb.HotelBookingId);
            modelBuilder.Entity<TourBooking>().HasKey(tb => tb.TourBookingId);

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

            modelBuilder.Entity<HotelBooking>()
                .HasOne(p => p.Hotel)
                .WithMany()
                .HasForeignKey(p => p.HotelId);

            modelBuilder.Entity<HotelBooking>()
                .HasOne(p => p.ApplicationUser)
                .WithMany()
                .HasForeignKey(p => p.UserId);

            modelBuilder.Entity<TourBooking>()
               .HasOne(p => p.Tour)
               .WithMany()
               .HasForeignKey(p => p.TourId);

            modelBuilder.Entity<TourBooking>()
                .HasOne(p => p.ApplicationUser)
                .WithMany()
                .HasForeignKey(p => p.UserId);

            base.OnModelCreating(modelBuilder);
        }
    }
}

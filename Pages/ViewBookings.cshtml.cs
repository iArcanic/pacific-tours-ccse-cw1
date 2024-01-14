using asp_net_core_web_app_authentication_authorisation.Models;
using asp_net_core_web_app_authentication_authorisation.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace asp_net_core_web_app_authentication_authorisation.Pages
{
    public class ViewBookingsModel : PageModel
    {
        [BindProperty]
        public ViewBookingsTableModel ViewBookingsTable { get; set; }

        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public ViewBookingsModel(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            ViewBookingsTable = new ViewBookingsTableModel();
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public class ViewBookingsTableModel
        {
            public List<HotelBooking> HotelBookingsList { get; set; } = new List<HotelBooking>();
            public List<TourBooking> TourBookingsList { get; set; } = new List<TourBooking>();
            public List<PackageBooking> PackageBookingsList { get; set; } = new List<PackageBooking>();
        }

        public async Task<IActionResult> OnGet()
        {
            var CurrentUser = await _userManager.GetUserAsync(User);

            var hotelBookingsList = await _dbContext.HotelBookings
                .Where(hb => hb.UserId.Equals(CurrentUser.Id) && hb.IsCancelled == false)
                .Include(hb => hb.Hotel)
                .ToListAsync();
            
            ViewBookingsTable.HotelBookingsList = hotelBookingsList;

            var tourBookingsList = await _dbContext.TourBookings
                .Where(tb => tb.UserId.Equals(CurrentUser.Id) && tb.IsCancelled == false)
                .Include(tb => tb.Tour)
                .ToListAsync();
             
            ViewBookingsTable.TourBookingsList = tourBookingsList;

            var packageBookingsList = await _dbContext.PackageBookings
                .Where(pb => pb.UserId.Equals(CurrentUser.Id) && pb.IsCancelled == false)
                .Include(pb => pb.Hotel)
                .Include(pb => pb.Tour)
                .ToListAsync();

            ViewBookingsTable.PackageBookingsList = packageBookingsList;

            return Page();
        }

        public async Task<IActionResult> OnPostHotelTableAsync(string command, string returnUrl = null)
        {
            if (command == "Cancel")
            {
                var HotelBookingId = new Guid(Request.Form["hotelBookingId"]);

                var hotelBooking = await _dbContext.HotelBookings
                    .Where(hb => hb.HotelBookingId == HotelBookingId)
                    .FirstOrDefaultAsync();

                hotelBooking.IsCancelled = true;

                await _dbContext.SaveChangesAsync();

                return RedirectToPage("/ViewBookings");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostTourTableAsync(string command, string returnUrl = null)
        {
            if (command == "Cancel")
            {
                var TourBookingId = new Guid(Request.Form["tourBookingId"]);

                var tourBooking = await _dbContext.TourBookings
                    .Where(tb => tb.TourBookingId == TourBookingId)
                    .FirstOrDefaultAsync();

                tourBooking.IsCancelled = true;

                await _dbContext.SaveChangesAsync();

                return RedirectToPage("/ViewBookings");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostPackageTableAsync(string command, string returnUrl = null)
        {
            if (command == "Cancel")
            {
                var PackageBookingId = new Guid(Request.Form["packageBookingId"]);

                var packageBooking = await _dbContext.PackageBookings
                    .Where(pb => pb.PackageBookingId == PackageBookingId)
                    .FirstOrDefaultAsync();

                packageBooking.IsCancelled = true;

                await _dbContext.SaveChangesAsync();

                return RedirectToPage("/ViewBookings");
            }

            return Page();
        }
    }
}

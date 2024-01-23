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

        // Application database context
        private readonly ApplicationDbContext _dbContext;

        // User manager to access current user
        private readonly UserManager<ApplicationUser> _userManager;

        public ViewBookingsModel(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            ViewBookingsTable = new ViewBookingsTableModel();
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public class ViewBookingsTableModel
        {
            // Lists to capture database objects
            public List<HotelBooking> HotelBookingsList { get; set; } = new List<HotelBooking>();
            public List<TourBooking> TourBookingsList { get; set; } = new List<TourBooking>();
            public List<PackageBooking> PackageBookingsList { get; set; } = new List<PackageBooking>();

            // Message for user feedback
            public string SuccessMessage { get; set; }
        }

        // When page loads
        public async Task<IActionResult> OnGet()
        {
            // Get current user
            var CurrentUser = await _userManager.GetUserAsync(User);

            // Get all hotels based on user's hotel bookings
            var hotelBookingsList = await _dbContext.HotelBookings
                .Where(hb => hb.UserId.Equals(CurrentUser.Id) && hb.IsCancelled == false)
                .Include(hb => hb.Hotel)
                .ToListAsync();

            // Display list of hotel bookings to UI
            ViewBookingsTable.HotelBookingsList = hotelBookingsList;

            // Get all tours based on user's tour bookings
            var tourBookingsList = await _dbContext.TourBookings
                .Where(tb => tb.UserId.Equals(CurrentUser.Id) && tb.IsCancelled == false)
                .Include(tb => tb.Tour)
                .ToListAsync();

            // Display list of tour bookings to UI
            ViewBookingsTable.TourBookingsList = tourBookingsList;

            // Get all hotels and tours (packages) based on user's package bookings
            var packageBookingsList = await _dbContext.PackageBookings
                .Where(pb => pb.UserId.Equals(CurrentUser.Id) && pb.IsCancelled == false)
                .Include(pb => pb.Hotel)
                .Include(pb => pb.Tour)
                .ToListAsync();

            // Display list of package  bookings to UI
            ViewBookingsTable.PackageBookingsList = packageBookingsList;

            // Get successMessage URL query parameter and display to UI
            ViewBookingsTable.SuccessMessage = Request.Query["successMessage"];

            return Page();
        }

        // On form submit
        public async Task<IActionResult> OnPostHotelTableAsync(string command, string returnUrl = null)
        {
            // If "Cancel" button is clicked
            if (command == "Cancel")
            {
                // Get hotel booking ID from form input
                var HotelBookingId = new Guid(Request.Form["hotelBookingId"]);

                // Get all hotel bookings based on ID
                var hotelBooking = await _dbContext.HotelBookings
                    .Where(hb => hb.HotelBookingId == HotelBookingId)
                    .FirstOrDefaultAsync();

                // Set cancelled property to "true" (soft deletion)
                hotelBooking.IsCancelled = true;

                // Save changes to database
                await _dbContext.SaveChangesAsync();

                // Redirect to "ViewBookings" page
                return RedirectToPage("/ViewBookings");
            }
            // If "Edit" button is clicked
            else
            {
                // Redirect to "EditHotelBooking" page and pass hotel booking ID in the URL
                return RedirectToPage("/EditHotelBooking", new 
                {
                    hotelBookingId = Request.Form["hotelBookingId"]
                });
            }
        }

        public async Task<IActionResult> OnPostTourTableAsync(string command, string returnUrl = null)
        {
            // If "Cancel" button is clicked
            if (command == "Cancel")
            {
                // Get tour booking ID from form input
                var TourBookingId = new Guid(Request.Form["tourBookingId"]);

                // Get all tour bookings based on ID
                var tourBooking = await _dbContext.TourBookings
                    .Where(tb => tb.TourBookingId == TourBookingId)
                    .FirstOrDefaultAsync();

                // Set cancelled property to "true" (soft deletion)
                tourBooking.IsCancelled = true;

                // Save changes to database
                await _dbContext.SaveChangesAsync();

                // Redirect to "ViewBookings" page
                return RedirectToPage("/ViewBookings");
            }
            // If "Edit" button is clicked
            else
            {
                // Redirect to "EditTourBooking" page and pass tour booking ID in the URL
                return RedirectToPage("/EditTourBooking", new
                {
                    tourBookingId = Request.Form["tourBookingId"]
                });
            }
        }

        public async Task<IActionResult> OnPostPackageTableAsync(string command, string returnUrl = null)
        {
            // If "Cancel" button is clicked
            if (command == "Cancel")
            {
                // Get package booking ID from form input
                var PackageBookingId = new Guid(Request.Form["packageBookingId"]);

                // Get all package bookings based on ID
                var packageBooking = await _dbContext.PackageBookings
                    .Where(pb => pb.PackageBookingId == PackageBookingId)
                    .FirstOrDefaultAsync();

                // Set cancelled property to "true" (soft deletion)
                packageBooking.IsCancelled = true;

                // Save changes to database
                await _dbContext.SaveChangesAsync();

                // Redirect to "ViewBookings" page
                return RedirectToPage("/ViewBookings");
            }
            // If "Edit" button is clicked
            else
            {
                // Redirect to "EditPackageBooking" page and pass package booking ID in the URL
                return RedirectToPage("/EditPackageBooking", new
                {
                    packageBookingId = Request.Form["packageBookingId"]
                });
            }
        }
    }
}

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
                .Where(hb => hb.UserId.Equals(CurrentUser.Id))
                .Include(hb => hb.Hotel)
                .ToListAsync();
            
            ViewBookingsTable.HotelBookingsList = hotelBookingsList;

            var tourBookingsList = await _dbContext.TourBookings
                .Where(tb => tb.UserId.Equals(CurrentUser.Id))
                .Include(tb => tb.Tour)
                .ToListAsync();

            ViewBookingsTable.TourBookingsList = tourBookingsList;

            return Page();
        }
    }
}

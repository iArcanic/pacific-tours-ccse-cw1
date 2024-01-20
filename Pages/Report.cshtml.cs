using asp_net_core_web_app_authentication_authorisation.Models;
using asp_net_core_web_app_authentication_authorisation.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace asp_net_core_web_app_authentication_authorisation.Pages
{
    public class ReportModel : PageModel
    {
        [BindProperty]
        public ReportTableModel ReportTable { get; set; }

        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReportModel(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            ReportTable = new ReportTableModel();
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public class ReportTableModel
        {
            public List<HotelBooking> HotelBookingsList { get; set; } = new List<HotelBooking>();
            public List<TourBooking> TourBookingsList { get; set; } = new List<TourBooking>();
            public List<PackageBooking> PackageBookingsList { get; set; } = new List<PackageBooking>();
        }

        public async Task<IActionResult> OnGet()
        {
            var hotelBookingsList = await _dbContext.HotelBookings
                .Include(hb => hb.Hotel)
                .Include(hb => hb.ApplicationUser)
                .ToListAsync();

            ReportTable.HotelBookingsList = hotelBookingsList;

            var tourBookingsList = await _dbContext.TourBookings
                .Include(tb => tb.Tour)
                .Include(hb => hb.ApplicationUser)
                .ToListAsync();

            ReportTable.TourBookingsList = tourBookingsList;

            var packageBookingsList = await _dbContext.PackageBookings
                .Include(pb => pb.Hotel)
                .Include(pb => pb.Tour)
                .Include(hb => hb.ApplicationUser)
                .ToListAsync();

            ReportTable.PackageBookingsList = packageBookingsList;

            return Page();
        }
    }
}

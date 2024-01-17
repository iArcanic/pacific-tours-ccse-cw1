using asp_net_core_web_app_authentication_authorisation.Models;
using asp_net_core_web_app_authentication_authorisation.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace asp_net_core_web_app_authentication_authorisation.Pages
{
    public class EditTourBookingModel : PageModel
    {
        [BindProperty]
        public EditBookingModel EditBooking { get; set; }

        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public EditTourBookingModel(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            EditBooking = new EditBookingModel();
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public class EditBookingModel
        {
            [Required(ErrorMessage = "Please select a tour start date")]
            [DataType(DataType.DateTime)]
            [Display(Name = "Tour start date")]
            public DateTime TourStartDate { get; set; }

            [Required(ErrorMessage = "Please select a tour end date")]
            [DataType(DataType.DateTime)]
            [Display(Name = "Tour end date")]
            public DateTime TourEndDate { get; set; }

            public List<Tour> ToursList { get; set; } = new List<Tour>();

            public string TourBookingId { get; set; }

            public string ErrorMessage { get; set; }
        }

        public async Task<IActionResult> OnGet()
        {
            var TourBookingIdValue = Request.Query["tourBookingId"];
            var TourBookingId = new Guid(TourBookingIdValue.ToString());

            var tourBooking = await _dbContext.TourBookings
                .Where(hb => hb.TourBookingId == TourBookingId)
                .Include(hb => hb.Tour)
                .FirstOrDefaultAsync();

            EditBooking.TourStartDate = tourBooking.TourStartDate;
            EditBooking.TourEndDate = tourBooking.TourEndDate;
            EditBooking.ToursList.Add(tourBooking.Tour);

            EditBooking.TourBookingId = TourBookingIdValue;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            EditBooking.ErrorMessage = null;

            var TourBookingIdValue = Request.Query["tourBookingId"];
            var TourBookingId = new Guid(TourBookingIdValue.ToString());

            var TourBooking = await _dbContext.TourBookings
                .Where(hb => hb.TourBookingId == TourBookingId)
                .Include(hb => hb.Tour)
                .FirstOrDefaultAsync();

            var CurrentUser = await _userManager.GetUserAsync(User);

            var TourAvailability = await _dbContext.TourAvailabilities
                .Where(ta =>
                    ta.TourId == TourBooking.TourId &&
                    ta.AvailableFrom <= EditBooking.TourStartDate &&
                    ta.AvailableTo >= EditBooking.TourEndDate)
                .Select(ha => ha.Tour)
                .Distinct()
                .ToListAsync();

            if (TourAvailability.Count == 1)
            {
                TourBooking.TourStartDate = EditBooking.TourStartDate;
                TourBooking.TourEndDate = EditBooking.TourEndDate;

                _dbContext.TourBookings.Update(TourBooking);
                await _dbContext.SaveChangesAsync();

                return RedirectToPage("/Payment", new
                {
                    bookingId = TourBookingIdValue,
                    bookingType = "tour"
                });
            }
            else
            {
                EditBooking.ErrorMessage = "Tours not available for selected dates";

                EditBooking.ToursList.Add(TourBooking.Tour);
                EditBooking.TourStartDate = EditBooking.TourStartDate;
                EditBooking.TourEndDate = EditBooking.TourEndDate;

                return Page();
            }
        }
    }
}

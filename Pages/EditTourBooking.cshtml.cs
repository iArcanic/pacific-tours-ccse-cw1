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

        // Application database context
        private readonly ApplicationDbContext _dbContext;

        // User manager to access current user
        private readonly UserManager<ApplicationUser> _userManager;

        public EditTourBookingModel(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            EditBooking = new EditBookingModel();
            _dbContext = dbContext;
            _userManager = userManager;
        }

        // Class containing variables binding to UI
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

        // When page loads
        public async Task<IActionResult> OnGet()
        {
            // Get tour booking ID from form
            var TourBookingIdValue = Request.Query["tourBookingId"];

            // Convert string value to GUID
            var TourBookingId = new Guid(TourBookingIdValue.ToString());

            // Find tour booking based on tour booking ID
            var tourBooking = await _dbContext.TourBookings
                .Where(hb => hb.TourBookingId == TourBookingId)
                .Include(hb => hb.Tour)
                .FirstOrDefaultAsync();

            // Set the database object's properties to UI values
            EditBooking.TourStartDate = tourBooking.TourStartDate;
            EditBooking.TourEndDate = tourBooking.TourEndDate;

            // Add to database
            EditBooking.ToursList.Add(tourBooking.Tour);

            // Set UI booking ID to model's tour booking ID
            EditBooking.TourBookingId = TourBookingIdValue;

            return Page();
        }

        // On form submit
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            // Clear error message each time
            EditBooking.ErrorMessage = null;

            // Get tour booking ID from form
            var TourBookingIdValue = Request.Query["tourBookingId"];

            // Convert string value to GUID
            var TourBookingId = new Guid(TourBookingIdValue.ToString());

            // Find tour booking based on tour booking ID
            var TourBooking = await _dbContext.TourBookings
                .Where(hb => hb.TourBookingId == TourBookingId)
                .Include(hb => hb.Tour)
                .FirstOrDefaultAsync();

            // Get current user
            var CurrentUser = await _userManager.GetUserAsync(User);

            // Get tour based on tour booking ID, selected dates, and available spaces
            var TourAvailability = await _dbContext.TourAvailabilities
                .Where(ta =>
                    ta.TourId == TourBooking.TourId &&
                    ta.AvailableFrom <= EditBooking.TourStartDate &&
                    ta.AvailableTo >= EditBooking.TourEndDate &&
                    ta.Tour.AvailableSpaces > 0)
                .Select(ha => ha.Tour)
                .Distinct()
                .ToListAsync();

            // If tour is found
            if (TourAvailability.Count == 1)
            {
                // Set tour database object properties to model variables
                TourBooking.TourStartDate = EditBooking.TourStartDate;
                TourBooking.TourEndDate = EditBooking.TourEndDate;

                // Update database tour booking object
                _dbContext.TourBookings.Update(TourBooking);

                // Decrement available space
                TourBooking.Tour.AvailableSpaces -= 1;

                // Save to database
                await _dbContext.SaveChangesAsync();

                // Redirect to "Payment" page, passing tour booking ID and booking type
                return RedirectToPage("/Payment", new
                {
                    bookingId = TourBookingIdValue,
                    bookingType = "tour"
                });
            }
            // If tours are not found
            else
            {
                // Display error message to UI
                EditBooking.ErrorMessage = "Tours not available for selected dates";

                // Add tour booking object to UI tour list
                EditBooking.ToursList.Add(TourBooking.Tour);

                // Set tour database object properties to model variables
                EditBooking.TourStartDate = EditBooking.TourStartDate;
                EditBooking.TourEndDate = EditBooking.TourEndDate;

                return Page();
            }
        }
    }
}

using asp_net_core_web_app_authentication_authorisation.Models;
using asp_net_core_web_app_authentication_authorisation.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace asp_net_core_web_app_authentication_authorisation.Pages
{
    public class EditHotelBookingModel : PageModel
    {
        [BindProperty]
        public EditBookingModel EditBooking { get; set; }

        // Application database context
        private readonly ApplicationDbContext _dbContext;

        // User manager to access current user
        private readonly UserManager<ApplicationUser> _userManager;

        // Class containing variables binding to UI
        public EditHotelBookingModel(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            EditBooking = new EditBookingModel();
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public class EditBookingModel
        {
            [Required(ErrorMessage = "Please select a check-in date")]
            [DataType(DataType.DateTime)]
            [Display(Name = "Check in date")]
            public DateTime CheckInDate { get; set; }

            [Required(ErrorMessage = "Please select a check-out date")]
            [DataType(DataType.DateTime)]
            [Display(Name = "Check out date")]
            public DateTime CheckOutDate { get; set; }

            public string RoomType { get; set; }

            public List<Hotel> HotelsList { get; set; } = new List<Hotel>();

            public string HotelBookingId { get; set; }

            public string ErrorMessage { get; set; }
        }

        // When page loads
        public async Task<IActionResult> OnGet()
        {
            // Get hotel booking ID from form
            var HotelBookingIdValue = Request.Query["hotelBookingId"];

            // Convert string value to GUID
            var HotelBookingId = new Guid(HotelBookingIdValue.ToString());

            // Find hotel booking based on pachotelkage booking ID
            var hotelBooking = await _dbContext.HotelBookings
                .Where(hb => hb.HotelBookingId == HotelBookingId)
                .Include(hb => hb.Hotel)
                .FirstOrDefaultAsync();

            // Set the database object's properties to UI values
            EditBooking.CheckInDate = hotelBooking.CheckInDate;
            EditBooking.CheckOutDate = hotelBooking.CheckOutDate;
            EditBooking.RoomType = hotelBooking.Hotel.RoomType;

            // Add hotel booking object to UI hotel list
            EditBooking.HotelsList.Add(hotelBooking.Hotel);

            // Set UI booking ID to model's hotel booking ID
            EditBooking.HotelBookingId = HotelBookingIdValue;

            return Page();
        }

        // On form submit
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            // Clear error message each time
            EditBooking.ErrorMessage = null;

            // Get tour booking ID from form
            var HotelBookingIdValue = Request.Query["hotelBookingId"];

            // Convert string value to GUID
            var HotelBookingId = new Guid(HotelBookingIdValue.ToString());

            // Find hotel booking based on hotel booking ID
            var HotelBooking = await _dbContext.HotelBookings
                .Where(hb => hb.HotelBookingId == HotelBookingId)
                .Include(hb => hb.Hotel)
                .FirstOrDefaultAsync();

            // Get current user
            var CurrentUser = await _userManager.GetUserAsync(User);

            // Get hotel based on hotel booking ID, selected dates, and available spaces
            var HotelAvailability = await _dbContext.HotelAvailabilities
                .Where(ha =>
                    ha.HotelId == HotelBooking.HotelId &&
                    ha.AvailableFrom <= EditBooking.CheckInDate &&
                    ha.AvailableTo >= EditBooking.CheckOutDate &&
                    ha.Hotel.AvailableSpaces > 0)
                .Select(ha => ha.Hotel)
                .Distinct()
                .ToListAsync();

            // If hotel is found
            if (HotelAvailability.Count == 1)
            {
                // Set hotel booking database object properties to model variables
                HotelBooking.CheckInDate = EditBooking.CheckInDate;
                HotelBooking.CheckOutDate = EditBooking.CheckOutDate;

                // Update database hotel booking object
                _dbContext.HotelBookings.Update(HotelBooking);

                // Decrement available spaces
                HotelBooking.Hotel.AvailableSpaces -= 1;

                // Save to database
                await _dbContext.SaveChangesAsync();

                // Redirect to "Payment" page, passing hotel booking ID and booking type
                return RedirectToPage("/Payment", new
                {
                    bookingId = HotelBookingIdValue,
                    bookingType = "hotel"
                });
            }
            // If hotel is not found
            else
            {
                // Display error message to UI
                EditBooking.ErrorMessage = "Hotels not available for selected dates";

                // Add hotel booking object to UI hotel list
                EditBooking.HotelsList.Add(HotelBooking.Hotel);

                // Set hotel database object properties to model variables
                EditBooking.CheckInDate = EditBooking.CheckInDate;
                EditBooking.CheckOutDate = EditBooking.CheckOutDate;
                EditBooking.RoomType = HotelBooking.Hotel.RoomType;

                return Page();
            }
        }
    }
}

using asp_net_core_web_app_authentication_authorisation.Models;
using asp_net_core_web_app_authentication_authorisation.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace asp_net_core_web_app_authentication_authorisation.Pages
{
    public class EditPackageBookingModel : PageModel
    {
        [BindProperty]
        public EditBookingModel EditBooking { get; set; }

        // Application database context
        private readonly ApplicationDbContext _dbContext;

        // User manager to access current user
        private readonly UserManager<ApplicationUser> _userManager;

        public EditPackageBookingModel(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            EditBooking = new EditBookingModel();
            _dbContext = dbContext;
            _userManager = userManager;
        }

        // Class containing variables binding to UI
        public class EditBookingModel
        {
            public string PackageBookingId { get; set; }

            [Required(ErrorMessage = "Please select a check-in date")]
            [DataType(DataType.DateTime)]
            [Display(Name = "Check in date")]
            public DateTime CheckInDate { get; set; }

            [Required(ErrorMessage = "Please select a check-out date")]
            [DataType(DataType.DateTime)]
            [Display(Name = "Check out date")]
            public DateTime CheckOutDate { get; set; }

            [Required(ErrorMessage = "Please select a room type")]
            [DataType(DataType.Text)]
            [Display(Name = "Room type")]
            public string RoomType { get; set; }

            public List<SelectListItem> RoomTypes { get; set; } = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = "single",
                    Text = "Single"
                },
                new SelectListItem
                {
                    Value = "double",
                    Text = "Double"
                },
                new SelectListItem
                {
                    Value = "family suite",
                    Text = "Family Suite"
                }
            };

            public List<Hotel> HotelsList { get; set; } = new List<Hotel>();

            [Required(ErrorMessage = "Please select a tour start date")]
            [DataType(DataType.DateTime)]
            [Display(Name = "Tour start date")]
            public DateTime TourStartDate { get; set; }

            [Required(ErrorMessage = "Please select a tour end date")]
            [DataType(DataType.DateTime)]
            [Display(Name = "Tour end date")]
            public DateTime TourEndDate { get; set; }

            public List<Tour> ToursList { get; set; } = new List<Tour>();

            public string ErrorMessage { get; set; }
        }

        // When page loads
        public async Task<IActionResult> OnGet()
        {
            // Get package booking ID from form
            var PackageBookingIdValue = Request.Query["packageBookingId"];

            // Convert string value to GUID
            var PackageBookingId = new Guid(PackageBookingIdValue.ToString());

            // Find package booking based on package booking ID
            var packageBooking = await _dbContext.PackageBookings
                .Where(pb => pb.PackageBookingId == PackageBookingId)
                .Include(pb => pb.Hotel)
                .Include(pb => pb.Tour)
                .FirstOrDefaultAsync();

            // Set the database object's properties to UI values
            EditBooking.CheckInDate = packageBooking.CheckInDate;
            EditBooking.CheckOutDate = packageBooking.CheckOutDate;
            EditBooking.RoomType = packageBooking.Hotel.RoomType;

            // Add hotel booking object to UI hotel list
            EditBooking.HotelsList.Add(packageBooking.Hotel);

            // Set the database object's properties to UI values
            EditBooking.TourStartDate = packageBooking.TourStartDate;
            EditBooking.TourEndDate = packageBooking.TourEndDate;

            // Add tour booking object to UI hotel list
            EditBooking.ToursList.Add(packageBooking.Tour);

            // Set UI booking ID to model's package booking ID
            EditBooking.PackageBookingId = PackageBookingIdValue;

            return Page();
        }

        // On form submit
        public async Task<IActionResult> OnPostAsync()
        {
            // Clear error message each time
            EditBooking.ErrorMessage = null;

            // Get tour booking ID from form
            var PackageBookingIdValue = Request.Query["packageBookingId"];

            // Convert string value to GUID
            var PackageBookingId = new Guid(PackageBookingIdValue.ToString());

            // Find package booking based on pacakge booking ID
            var packageBooking = await _dbContext.PackageBookings
                .Where(pb => pb.PackageBookingId == PackageBookingId)
                .Include(pb => pb.Hotel)
                .Include(pb => pb.Tour)
                .FirstOrDefaultAsync();

            // Get current user
            var CurrentUser = await _userManager.GetUserAsync(User);

            // Get hotel based on hotel booking ID, selected dates, and available spaces
            var hotelAvailability = await _dbContext.HotelAvailabilities
                .Where(ha =>
                    ha.HotelId == packageBooking.HotelId &&
                    ha.AvailableFrom <= EditBooking.CheckInDate &&
                    ha.AvailableTo >= EditBooking.CheckOutDate &&
                    ha.Hotel.AvailableSpaces > 0)
                .Select(ha => ha.Hotel)
                .Distinct()
                .ToListAsync();

            // Get tour based on tour booking ID, selected dates, and available spaces
            var tourAvailability = await _dbContext.TourAvailabilities
                .Where(ta =>
                    ta.TourId == packageBooking.TourId &&
                    ta.AvailableFrom <= EditBooking.TourStartDate &&
                    ta.AvailableTo >= EditBooking.TourEndDate &&
                    ta.Tour.AvailableSpaces > 0)
                .Select(ha => ha.Tour)
                .Distinct()
                .ToListAsync();

            // If hotel and tour is found
            if (hotelAvailability.Count == 1 && tourAvailability.Count == 1)
            {
                // Set package booking database object properties to model variables
                packageBooking.CheckInDate = EditBooking.CheckInDate;
                packageBooking.CheckOutDate = EditBooking.CheckOutDate;
                packageBooking.TourStartDate = EditBooking.TourStartDate;
                packageBooking.TourEndDate = EditBooking.TourEndDate;

                // Update database tour booking object
                _dbContext.PackageBookings.Update(packageBooking);

                // Decrement available spaces
                packageBooking.Hotel.AvailableSpaces -= 1;
                packageBooking.Tour.AvailableSpaces -= 1;

                // Save to database
                await _dbContext.SaveChangesAsync();

                // Redirect to "Payment" page, passing package booking ID and booking type
                return RedirectToPage("/Payment", new
                {
                    bookingId = PackageBookingIdValue,
                    bookingType = "package"
                });
            }
            // If hotel and tour is not found
            else
            {
                // Display error message to UI
                EditBooking.ErrorMessage = "Hotels and/or Tours not available for selected dates";
                
                // Add hotel booking object to UI hotel list
                EditBooking.HotelsList.Add(packageBooking.Hotel);

                // Set hotel database object properties to model variables
                EditBooking.CheckInDate = EditBooking.CheckInDate;
                EditBooking.CheckOutDate = EditBooking.CheckOutDate;
                EditBooking.RoomType = packageBooking.Hotel.RoomType;

                // Add tour booking object to UI tour list
                EditBooking.ToursList.Add(packageBooking.Tour);

                // Set tour database object properties to model variables
                EditBooking.TourStartDate = EditBooking.TourStartDate;
                EditBooking.TourEndDate = EditBooking.TourEndDate;

                return Page();
            }
        }
    }
}

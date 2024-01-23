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
    public class BookingsModel : PageModel
    {
        [BindProperty]
        public HotelSearchModel HotelSearch { get; set; }

        [BindProperty]
        public TourSearchModel TourSearch { get; set; }

        [BindProperty]
        public PackageBookModel PackageBook { get; set; }

        // Application database context
        private readonly ApplicationDbContext _dbContext;

        // User manager to access current user
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingsModel(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        { 
            HotelSearch = new HotelSearchModel();
            TourSearch = new TourSearchModel();
            PackageBook = new PackageBookModel();
            _dbContext = dbContext;
            _userManager = userManager;
        }

        // Class containing hotel variables binding to UI
        public class HotelSearchModel
        {
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
            public string RoomType { get; set; } = "Single";

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
        }

        // Class containing tour variables binding to UI
        public class TourSearchModel
        {
            [Required(ErrorMessage = "Please select a tour start date")]
            [DataType(DataType.DateTime)]
            [Display(Name = "Tour start date")]
            public DateTime TourStartDate { get; set; }

            [Required(ErrorMessage = "Please select a tour end date")]
            [DataType(DataType.DateTime)]
            [Display(Name = "Tour end date")]
            public DateTime TourEndDate { get; set; }

            public List<String> AvailableTours { get; set; } = new List<string>();

            public List<Tour> ToursList { get; set; } = new List<Tour>();
        }

        // Class containing package variables binding to UI
        public class PackageBookModel
        {
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
            public string RoomType { get; set; } = "Single";

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
            public List<HotelDiscount> HotelDiscountsList { get; set; } = new List<HotelDiscount>();
        }

        // When page loads
        public async Task<IActionResult> OnGet()
        {
            // Set hotel discounts to hotel discounts list in UI
            PackageBook.HotelDiscountsList = await _dbContext.HotelDiscounts.ToListAsync();
 
            return Page();
        }

        // On hotel form submit
        public async Task<IActionResult> OnPostHotelSearchAsync(string command, string returnUrl = null)
        {
            // Check if the page is valid
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // If "Search" button is clicked
            if (command == "Search")
            {
                // Get all available hotels
                var availableHotels = await _dbContext.HotelAvailabilities
                    .Where(ha =>
                        ha.AvailableFrom <= HotelSearch.CheckInDate &&
                        ha.AvailableTo >= HotelSearch.CheckOutDate &&
                        ha.Hotel.RoomType == HotelSearch.RoomType &&
                        ha.Hotel.AvailableSpaces > 0)
                    .Select(ha => ha.Hotel)
                    .Distinct()
                    .ToListAsync();

                // Bind available hotels to UI hotels list
                HotelSearch.HotelsList = availableHotels;

                return Page();
            }
            // If "Book" button is clicked
            else
            {
                // Get hotel ID from form
                var SelectedHotelId = new Guid(Request.Form["hotels"]);

                // Get current user
                var CurrentUser = await _userManager.GetUserAsync(User);

                // Get hotel based on hotel ID
                Hotel SelectedHotel = await _dbContext.Hotels.FindAsync(SelectedHotelId);

                // Make a new hotel booking record
                var hotelBooking = new HotelBooking
                {
                    HotelBookingId = new Guid(),
                    HotelId = SelectedHotelId,
                    UserId = CurrentUser.Id,
                    CheckInDate = HotelSearch.CheckInDate,
                    CheckOutDate = HotelSearch.CheckOutDate,
                    Hotel = SelectedHotel,
                    ApplicationUser = CurrentUser
                };

                // Add to database
                _dbContext.HotelBookings.Add(hotelBooking);

                // Decrement available spaces
                hotelBooking.Hotel.AvailableSpaces -= 1;


                // Save changes to database
                await _dbContext.SaveChangesAsync();

                // Redirect to "Payment" page, passing hotel booking ID and booking type
                return RedirectToPage("/Payment", new
                {
                    bookingId = hotelBooking.HotelBookingId.ToString(),
                    bookingType = "hotel"
                });
            }
        }

        // On tour form submit
        public async Task<IActionResult> OnPostTourSearchAsync(string command, string returnUrl = null)
        {
            // Check if the page is valid
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // If the "Search" button is clicked
            if (command == "Search")
            {
                // Get all available tours
                var availableTours = await _dbContext.TourAvailabilities
                .Where(ta =>
                    ta.AvailableFrom <= TourSearch.TourStartDate && 
                    ta.AvailableTo >= TourSearch.TourEndDate &&
                    ta.Tour.AvailableSpaces > 0)
                .Select(ta => ta.Tour)
                .Distinct()
                .ToListAsync();

                // Bind available tours to UI tours list
                TourSearch.ToursList = availableTours;

                return Page();
            }
            else
            {
                // Get hotel ID from form
                var SelectedTourId = new Guid(Request.Form["tours"]);

                // Get current user
                var CurrentUser = await _userManager.GetUserAsync(User);

                // Get tour based on tour ID
                Tour SelectedTour = await _dbContext.Tours.FindAsync(SelectedTourId);

                // Make a new tour booking record
                var tourBooking = new TourBooking
                {
                    TourBookingId = new Guid(),
                    TourId = SelectedTourId,
                    UserId = CurrentUser.Id,
                    TourStartDate = TourSearch.TourStartDate,
                    TourEndDate = TourSearch.TourEndDate,
                    Tour = SelectedTour,
                    ApplicationUser = CurrentUser
                };

                // Add to database
                _dbContext.TourBookings.Add(tourBooking);

                // Decrement available spaces
                tourBooking.Tour.AvailableSpaces -= 1;

                // Save changes to database
                await _dbContext.SaveChangesAsync();

                // Redirect to "Payment" page, passing tour booking ID and booking type
                return RedirectToPage("/Payment", new
                {
                    bookingId = tourBooking.TourBookingId.ToString(),
                    bookingType = "tour"
                });
            }
        }

        // On package form submit
        public async Task<IActionResult> OnPostPackageBookAsync(string command, string returnUrl = null)
        {
            // Check if the page is valid
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Get hotel discounts and bind to hotel discounts UI list
            PackageBook.HotelDiscountsList = await _dbContext.HotelDiscounts.ToListAsync();

            // If "Search" button is clicked
            if (command == "Search")
            {
                // Get all available hotels
                var availableHotels = await _dbContext.HotelAvailabilities
                .Where(ha =>
                    ha.AvailableFrom <= PackageBook.CheckInDate && 
                    ha.AvailableTo >= PackageBook.CheckOutDate &&
                    ha.Hotel.RoomType == PackageBook.RoomType &&
                    ha.Hotel.AvailableSpaces > 0)
                .Select(ha => ha.Hotel)
                .Distinct()
                .ToListAsync();

                // Bind available hotel to UI hotel list
                PackageBook.HotelsList = availableHotels;

                // Get all available tours
                var availableTours = await _dbContext.TourAvailabilities
                    .Where(ta =>
                        ta.AvailableFrom <= PackageBook.TourStartDate && 
                        ta.AvailableTo >= PackageBook.TourEndDate &&
                        ta.Tour.AvailableSpaces > 0)
                    .Select(ta => ta.Tour)
                    .Distinct()
                    .ToListAsync();

                // Bind available tours to UI tours list
                PackageBook.ToursList = availableTours;

                return Page();
            }
            // If "Book" button is clicked
            else
            {
                // Get current user
                var CurrentUser = await _userManager.GetUserAsync(User);
                
                // Get hotel ID from form
                var SelectedHotelId = new Guid(Request.Form["packageHotelsDropdown"]);

                // Get hotel from hotel ID
                Hotel SelectedHotel = await _dbContext.Hotels.FindAsync(SelectedHotelId);

                // Get tour ID from form
                var SelectedTourId = new Guid(Request.Form["packageToursDropdown"]);

                // Get tour from tour ID
                Tour SelectedTour = await _dbContext.Tours.FindAsync(SelectedTourId);

                // Make new package booking
                var packageBooking = new PackageBooking
                {
                    PackageBookingId = new Guid(),
                    UserId = CurrentUser.Id,
                    HotelId = SelectedHotelId,
                    CheckInDate = PackageBook.CheckInDate,
                    CheckOutDate = PackageBook.CheckOutDate,
                    TourId = SelectedTourId,
                    TourStartDate = PackageBook.TourStartDate,
                    TourEndDate = PackageBook.TourEndDate,
                    Hotel = SelectedHotel,
                    Tour = SelectedTour,
                    ApplicationUser = CurrentUser
                };

                // Add to database
                _dbContext.PackageBookings.Add(packageBooking);

                // Decrement available spaces
                packageBooking.Hotel.AvailableSpaces -= 1;
                packageBooking.Tour.AvailableSpaces -= 1;

                // Save changes to database
                await _dbContext.SaveChangesAsync();

                // Redirect to "Payment" page, passing package booking ID and booking type
                return RedirectToPage("/Payment", new
                {
                    bookingId = packageBooking.PackageBookingId.ToString(),
                    bookingType = "package"
                });
            }
        }
    }
}

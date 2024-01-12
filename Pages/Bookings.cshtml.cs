using asp_net_core_web_app_authentication_authorisation.Models;
using asp_net_core_web_app_authentication_authorisation.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Media;

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

        private readonly ApplicationDbContext _dbContext;

        private readonly UserManager<ApplicationUser> _userManager;

        public BookingsModel(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        { 
            HotelSearch = new HotelSearchModel();
            TourSearch = new TourSearchModel();
            PackageBook = new PackageBookModel();
            _dbContext = dbContext;
            _userManager = userManager;
        }

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
        }

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

            public List<String> AvailableHotels { get; set; } = new List<string>();

            [Required(ErrorMessage = "Please select a tour start date")]
            [DataType(DataType.DateTime)]
            [Display(Name = "Tour start date")]
            public DateTime TourStartDate { get; set; }

            [Required(ErrorMessage = "Please select a tour end date")]
            [DataType(DataType.DateTime)]
            [Display(Name = "Tour end date")]
            public DateTime TourEndDate { get; set; }

            public List<String> AvailableTours { get; set; } = new List<string>();
        }

        public async Task<IActionResult> OnPostHotelSearchAsync(string command, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (command == "Search")
            {
                var availableHotels = await _dbContext.HotelAvailabilities
                    .Where(ha =>
                        ha.AvailableFrom <= HotelSearch.CheckInDate && ha.AvailableTo >= HotelSearch.CheckOutDate)
                    .Select(ha => ha.Hotel)
                    .Distinct()
                    .ToListAsync();

                HotelSearch.HotelsList = availableHotels;

                return Page();
            }
            else
            {
                var SelectedHotelId = new Guid(Request.Form["hotels"]);

                var currentUser = await _userManager.GetUserAsync(User);

                Hotel selectedHotel = await _dbContext.Hotels.FindAsync(SelectedHotelId);

                var hotelBooking = new HotelBooking
                {
                    HotelBookingId = new Guid(),
                    HotelId = SelectedHotelId,
                    UserId = currentUser.Id,
                    CheckInDate = HotelSearch.CheckInDate,
                    CheckOutDate = HotelSearch.CheckOutDate,
                    Hotel = selectedHotel,
                    ApplicationUser = currentUser
                };

                _dbContext.HotelBookings.Add(hotelBooking);
                await _dbContext.SaveChangesAsync();

                return Page();
            }
        }

        public async Task<IActionResult> OnPostTourSearchAsync(string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var availableTours = await _dbContext.TourAvailabilities
                .Where(ta =>
                    ta.AvailableFrom <= TourSearch.TourStartDate && ta.AvailableTo >= TourSearch.TourEndDate)
                .Select(ta => ta.Tour.Name)
                .Distinct()
                .ToListAsync();

            TourSearch.AvailableTours = availableTours;

            return Page();
        }

        public async Task<IActionResult> OnPostPackageBookAsync(string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var availableHotels = await _dbContext.HotelAvailabilities
                .Where(ha =>
                    ha.AvailableFrom <= PackageBook.CheckInDate && ha.AvailableTo >= PackageBook.CheckOutDate)
                .Select(ha => ha.Hotel.Name)
                .Distinct()
                .ToListAsync();

            PackageBook.AvailableHotels = availableHotels;

            var availableTours = await _dbContext.TourAvailabilities
                .Where(ta =>
                    ta.AvailableFrom <= PackageBook.TourStartDate && ta.AvailableTo >= PackageBook.TourEndDate)
                .Select(ta => ta.Tour.Name)
                .Distinct()
                .ToListAsync();

            PackageBook.AvailableTours = availableTours;

            return Page();
        }
    }
}

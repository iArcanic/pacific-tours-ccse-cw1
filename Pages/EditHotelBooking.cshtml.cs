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
    public class EditHotelBookingModel : PageModel
    {
        [BindProperty]
        public HotelSearchModel HotelSearch { get; set; }

        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public EditHotelBookingModel(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            HotelSearch = new HotelSearchModel();
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
        }

        public async Task<IActionResult> OnGet()
        {

            var HotelBookingIdValue = Request.Query["hotelBookingId"];

            var HotelBookingId = new Guid(HotelBookingIdValue.ToString());

            var hotelBooking = await _dbContext.HotelBookings
                .Where(hb => hb.HotelBookingId == HotelBookingId)
                .Include(hb => hb.Hotel)
                .FirstOrDefaultAsync();

            HotelSearch.CheckInDate = hotelBooking.CheckInDate;
            HotelSearch.CheckOutDate = hotelBooking.CheckOutDate;
            HotelSearch.RoomType = hotelBooking.Hotel.RoomType;
            HotelSearch.HotelsList.Add(hotelBooking.Hotel);

            return Page();
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
                var CurrentUser = await _userManager.GetUserAsync(User);
                Hotel SelectedHotel = await _dbContext.Hotels.FindAsync(SelectedHotelId);

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

                _dbContext.HotelBookings.Add(hotelBooking);
                await _dbContext.SaveChangesAsync();

                return Page();
            }
        }
    }
}

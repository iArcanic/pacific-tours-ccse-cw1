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

        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

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

        public async Task<IActionResult> OnGet()
        {
            var HotelBookingIdValue = Request.Query["hotelBookingId"];
            var HotelBookingId = new Guid(HotelBookingIdValue.ToString());

            var hotelBooking = await _dbContext.HotelBookings
                .Where(hb => hb.HotelBookingId == HotelBookingId)
                .Include(hb => hb.Hotel)
                .FirstOrDefaultAsync();

            EditBooking.CheckInDate = hotelBooking.CheckInDate;
            EditBooking.CheckOutDate = hotelBooking.CheckOutDate;
            EditBooking.RoomType = hotelBooking.Hotel.RoomType;
            EditBooking.HotelsList.Add(hotelBooking.Hotel);

            EditBooking.HotelBookingId = HotelBookingIdValue;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            EditBooking.ErrorMessage = null;

            var HotelBookingIdValue = Request.Query["hotelBookingId"];
            var HotelBookingId = new Guid(HotelBookingIdValue.ToString());

            var HotelBooking = await _dbContext.HotelBookings
                .Where(hb => hb.HotelBookingId == HotelBookingId)
                .Include(hb => hb.Hotel)
                .FirstOrDefaultAsync();

            var CurrentUser = await _userManager.GetUserAsync(User);

            var HotelAvailability = await _dbContext.HotelAvailabilities
                .Where(ha =>
                    ha.HotelId == HotelBooking.HotelId &&
                    ha.AvailableFrom <= EditBooking.CheckInDate &&
                    ha.AvailableTo >= EditBooking.CheckOutDate)
                .Select(ha => ha.Hotel)
                .Distinct()
                .ToListAsync();

            if (HotelAvailability.Count == 1)
            {
                HotelBooking.CheckInDate = EditBooking.CheckInDate;
                HotelBooking.CheckOutDate = EditBooking.CheckOutDate;

                _dbContext.HotelBookings.Update(HotelBooking);
                await _dbContext.SaveChangesAsync();

                return RedirectToPage("/ViewBookings", new
                {
                    successMessage = "Your booking has been successfully modified!"
                });
            }
            else
            {
                EditBooking.ErrorMessage = "Hotels not available for selected dates";

                EditBooking.HotelsList.Add(HotelBooking.Hotel);
                EditBooking.CheckInDate = EditBooking.CheckInDate;
                EditBooking.CheckOutDate = EditBooking.CheckOutDate;
                EditBooking.RoomType = HotelBooking.Hotel.RoomType;

                return Page();
            }
        }
    }
}

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

        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public EditPackageBookingModel(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            EditBooking = new EditBookingModel();
            _dbContext = dbContext;
            _userManager = userManager;
        }

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

        public async Task<IActionResult> OnGet()
        {
            var PackageBookingIdValue = Request.Query["packageBookingId"];
            var PackageBookingId = new Guid(PackageBookingIdValue.ToString());

            var packageBooking = await _dbContext.PackageBookings
                .Where(pb => pb.PackageBookingId == PackageBookingId)
                .Include(pb => pb.Hotel)
                .Include(pb => pb.Tour)
                .FirstOrDefaultAsync();

            EditBooking.CheckInDate = packageBooking.CheckInDate;
            EditBooking.CheckOutDate = packageBooking.CheckOutDate;
            EditBooking.RoomType = packageBooking.Hotel.RoomType;
            EditBooking.HotelsList.Add(packageBooking.Hotel);

            EditBooking.TourStartDate = packageBooking.TourStartDate;
            EditBooking.TourEndDate = packageBooking.TourEndDate;
            EditBooking.ToursList.Add(packageBooking.Tour);

            EditBooking.PackageBookingId = PackageBookingIdValue;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            EditBooking.ErrorMessage = null;

            var PackageBookingIdValue = Request.Query["packageBookingId"];
            var PackageBookingId = new Guid(PackageBookingIdValue.ToString());

            var packageBooking = await _dbContext.PackageBookings
                .Where(pb => pb.PackageBookingId == PackageBookingId)
                .Include(pb => pb.Hotel)
                .Include(pb => pb.Tour)
                .FirstOrDefaultAsync();

            var CurrentUser = await _userManager.GetUserAsync(User);

            var hotelAvailability = await _dbContext.HotelAvailabilities
                .Where(ha =>
                    ha.HotelId == packageBooking.HotelId &&
                    ha.AvailableFrom <= EditBooking.CheckInDate &&
                    ha.AvailableTo >= EditBooking.CheckOutDate)
                .Select(ha => ha.Hotel)
                .Distinct()
                .ToListAsync();

            var tourAvailability = await _dbContext.TourAvailabilities
                .Where(ta =>
                    ta.TourId == packageBooking.TourId &&
                    ta.AvailableFrom <= EditBooking.TourStartDate &&
                    ta.AvailableTo >= EditBooking.TourEndDate)
                .Select(ha => ha.Tour)
                .Distinct()
                .ToListAsync();

            if (hotelAvailability.Count == 1 && tourAvailability.Count == 1)
            {
                packageBooking.CheckInDate = EditBooking.CheckInDate;
                packageBooking.CheckOutDate = EditBooking.CheckOutDate;

                packageBooking.TourStartDate = EditBooking.TourStartDate;
                packageBooking.TourEndDate = EditBooking.TourEndDate;

                _dbContext.PackageBookings.Update(packageBooking);
                await _dbContext.SaveChangesAsync();

                return RedirectToPage("/ViewBookings", new
                {
                    successMessage = "Your Package booking has been successfully modified!"
                });
            }
            else
            {
                EditBooking.ErrorMessage = "Hotels and/or Tours not available for selected dates";

                EditBooking.HotelsList.Add(packageBooking.Hotel);
                EditBooking.CheckInDate = EditBooking.CheckInDate;
                EditBooking.CheckOutDate = EditBooking.CheckOutDate;
                EditBooking.RoomType = packageBooking.Hotel.RoomType;

                EditBooking.ToursList.Add(packageBooking.Tour);
                EditBooking.TourStartDate = EditBooking.TourStartDate;
                EditBooking.TourEndDate = EditBooking.TourEndDate;

                return Page();
            }
        }
    }
}

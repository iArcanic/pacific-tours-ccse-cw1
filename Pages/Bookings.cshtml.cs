using asp_net_core_web_app_authentication_authorisation.Services;
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
        public InputModel Input { get; set; }

        private readonly ApplicationDbContext _dbContext;

        public BookingsModel(ApplicationDbContext dbContext)
        { 
            Input = new InputModel();
            _dbContext = dbContext;
        }

        public class InputModel
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

            public string HotelActiveTab { get; set; } = "HotelsTabSelected";
            public string TourActiveTab { get; set; } = "ToursTabSelected";
            public string PackageActiveTab { get; set; } = "PackagesTabSelected";

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
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var availableHotels = await _dbContext.HotelAvailabilities
                .Where(ha => 
                    ha.AvailableFrom <= Input.CheckInDate && ha.AvailableTo >= Input.CheckOutDate)
                .Select(ha => ha.Hotel.Name)
                .Distinct()
                .ToListAsync();

            Input.AvailableHotels = availableHotels;

            return Page();
        }
    }
}

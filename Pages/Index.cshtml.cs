using asp_net_core_web_app_authentication_authorisation.Models;
using asp_net_core_web_app_authentication_authorisation.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace asp_net_core_web_app_authentication_authorisation.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public PricesTableModel PricesTable { get; set; }

        private readonly ILogger<IndexModel> _logger;

        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            PricesTable = new PricesTableModel();
            _dbContext = dbContext;
            _userManager = userManager;
            _logger = logger;
        }

        public class PricesTableModel
        {
            public List<Hotel> HotelList { get; set; } = new List<Hotel>();
            public List<Tour> TourList { get; set; } = new List<Tour>();
            public List<HotelDiscount> HotelDiscountList { get; set; } = new List<HotelDiscount>();
        }

        public async Task<IActionResult> OnGet()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var hotelsList = await _dbContext.Hotels
                .ToListAsync();

            PricesTable.HotelList = hotelsList;

            var toursList = await _dbContext.Tours
                .ToListAsync();

            PricesTable.TourList = toursList;

            var hotelDiscountsList = await _dbContext.HotelDiscounts
                .ToListAsync();

            PricesTable.HotelDiscountList = hotelDiscountsList;

            return Page();
        }
    }
}

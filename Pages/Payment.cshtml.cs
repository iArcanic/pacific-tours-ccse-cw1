using asp_net_core_web_app_authentication_authorisation.Models;
using asp_net_core_web_app_authentication_authorisation.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace asp_net_core_web_app_authentication_authorisation.Pages
{
    public class PaymentModel : PageModel
    {
        [BindProperty]
        public PaymentFormModel PaymentForm { get; set; }

        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public PaymentModel(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            PaymentForm = new PaymentFormModel();
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public class PaymentFormModel
        {
            [Required(ErrorMessage = "Please enter name on card")]
            [DataType(DataType.Text)]
            [Display(Name = "Card name")]
            public string CardName { get; set; }

            [Required(ErrorMessage = "Please enter card number")]
            [DataType(DataType.Text)]
            [Display(Name = "Card name")]
            [RegularExpression(@"^\d{16}$", ErrorMessage = "Invalid credit card number. Must be 16 digits.")]
            public string CardNumber { get; set; }

            [Required(ErrorMessage = "Please enter billing address")]
            [DataType(DataType.Text)]
            [Display(Name = "Billing address")]
            public string BillingAddress { get; set; }

            [Required(ErrorMessage = "Please input card expiry date")]
            [DataType(DataType.Date)]
            [Display(Name = "Expiry date")]
            public DateOnly CardExpiryDate { get; set; }

            [Required(ErrorMessage = "Please enter CVC number")]
            [Display(Name = "CVC number")]
            [RegularExpression(@"^\d{3,4}$", ErrorMessage = "Invalid CVC number")]
            public string CvcNumber { get; set; }

            public string ErrorMessage { get; set; } = "";
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var BookingId = new Guid(Request.Query["bookingId"]);
            var BookingType = Request.Query["bookingType"];

            if (BookingType == "hotel")
            {
                HotelBooking hotelBooking = await _dbContext.HotelBookings.FindAsync(BookingId);

                hotelBooking.IsPaid = true;
                await _dbContext.SaveChangesAsync();

                return RedirectToPage("/ViewBookings", new
                { 
                    successMessage = "Success!"
                });
            }
            else if (BookingType == "tour")
            {
                TourBooking tourBooking = await _dbContext.TourBookings.FindAsync(BookingId);

                tourBooking.IsPaid = true;
                await _dbContext.SaveChangesAsync();

                return RedirectToPage("/ViewBookings", new
                {
                    successMessage = "Success!"
                });
            }
            else if (BookingType == "package")
            {
                PackageBooking packageBooking = await _dbContext.PackageBookings.FindAsync(BookingId);

                packageBooking.IsPaid = true;
                await _dbContext.SaveChangesAsync();

                return RedirectToPage("/ViewBookings", new
                {
                    successMessage = "Success!"
                });
            }
            else
            {
                PaymentForm.ErrorMessage = null;

                PaymentForm.ErrorMessage = "A payment error has occured. Please try again later.";

                return Page();
            }
        }
    }
}

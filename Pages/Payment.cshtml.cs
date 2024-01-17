using asp_net_core_web_app_authentication_authorisation.Models;
using asp_net_core_web_app_authentication_authorisation.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
            [Display(Name = "Card name")]
            [CreditCard(ErrorMessage = "Invalid credit card number")]
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
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            return Page();
        }
    }
}

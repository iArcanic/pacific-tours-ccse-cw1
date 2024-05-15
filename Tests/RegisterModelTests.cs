using System.Threading.Tasks;
using asp_net_core_web_app_authentication_authorisation.Areas.Identity.Pages.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using asp_net_core_web_app_authentication_authorisation.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Moq;
using Xunit;

namespace YourNamespace.Tests
{
    public class RegisterModelTests
    {
        [Fact]
        public async Task OnPostAsync_ValidModelState_ReturnsRedirectToActionResult()
        {
            // Arrange
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            var signInManagerMock = new Mock<SignInManager<ApplicationUser>>(userManagerMock.Object, null, null, null, null, null, null);
            var loggerMock = new Mock<ILogger<RegisterModel>>();
            var emailSenderMock = new Mock<IEmailSender>();

            var model = new RegisterModel(userManagerMock.Object, null, signInManagerMock.Object, loggerMock.Object, emailSenderMock.Object)
            {
                Input = new RegisterModel.InputModel
                {
                    FirstName = "John",
                    LastName = "Doe",
                    PhoneNumber = "1234567890",
                    Address = "123 Main St",
                    PassportNumber = "ABC123",
                    Email = "john.doe@example.com",
                    Password = "P@ssw0rd",
                    ConfirmPassword = "P@ssw0rd"
                }
            };

            // Act
            var result = await model.OnPostAsync();

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
            var redirectToPageResult = (RedirectToPageResult)result;
            Assert.Equal("RegisterConfirmation", redirectToPageResult.PageName);
        }
    }
}

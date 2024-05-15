using asp_net_core_web_app_authentication_authorisation.Pages;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace asp_net_core_web_app_authentication_authorisation.Tests
{
    public class PrivacyModelTests
    {
        [Fact]
        public void OnGet_NoExceptionsThrown()
        {
            // Arrange
            var model = new PrivacyModel(NullLogger<PrivacyModel>.Instance);

            // Act & Assert
            try
            {
                model.OnGet();
            }
            catch (System.Exception ex)
            {
                // Fail the test if an exception is caught
                Assert.True(false, $"Unexpected exception thrown: {ex.Message}");
            }
        }
    }
}

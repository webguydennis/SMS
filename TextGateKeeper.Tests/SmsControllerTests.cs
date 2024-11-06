using Moq;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TextGateKeeper.Controllers;
using TextGateKeeper.Data;
using TextGateKeeper.Dtos;

namespace TextGateKeeper.Tests {

    [TestFixture]
    public class SmsControllerTests
    {
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<IConfigurationSection>? _mockConfigurationSection;
        private SmsController _smsController;
        private Mock<ITextMessageRepository> _mockTextMessageRepository;
        
        [SetUp]
        public void Setup()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            Mock<IConfigurationSection> maxLimitAccountSection = new Mock<IConfigurationSection>();
            Mock<IConfigurationSection> maxLimitPhoneSection = new Mock<IConfigurationSection>();
            Mock<IConfigurationSection> cutOffInDaysSection = new Mock<IConfigurationSection>();
            
            maxLimitAccountSection.Setup(s => s.Value).Returns("10");
            _mockConfiguration.Setup(c => c
                .GetSection("AppSettings:MaxLimitAccountPerSecond"))
                .Returns(maxLimitAccountSection.Object);

            maxLimitPhoneSection.Setup(s => s.Value).Returns("5");
            _mockConfiguration.Setup(c => c
                .GetSection("AppSettings:MaxLimitPhonePerSecond"))
                .Returns(maxLimitPhoneSection.Object);

             cutOffInDaysSection.Setup(s => s.Value).Returns("1");
            _mockConfiguration.Setup(c => c
                .GetSection("AppSettings:CutOffInDays"))
                .Returns(cutOffInDaysSection.Object);
            
            _mockTextMessageRepository = new Mock<ITextMessageRepository>();
            _smsController = new SmsController(_mockConfiguration.Object, _mockTextMessageRepository.Object);
        }

        [Test]
        public void SmsController_GetMaxLimitAccountFromAppSettings_return10()
        {
            // Arrange
            IConfiguration config = _mockConfiguration.Object;

            // Act
            var result = config.GetSection("AppSettings:MaxLimitAccountPerSecond").Value;            

            // Assert
             Assert.That(result, Is.EqualTo("10"));
        }

        [Test]
        public void SmsController_GetMaxLimitAccountFromAppSettings_return5()
        {
            // Arrange
            IConfiguration config = _mockConfiguration.Object;

            // Act
            var result = config.GetSection("AppSettings:MaxLimitPhonePerSecond").Value;            

            // Assert
             Assert.That(result, Is.EqualTo("5"));
        }

        [Test]
        public void SmsController_GetCutOffInDaysFromAppSettings_return1()
        {
            // Arrange
            IConfiguration config = _mockConfiguration.Object;

            // Act
            var result = config.GetSection("AppSettings:CutOffInDays").Value;            

            // Assert
             Assert.That(result, Is.EqualTo("1"));
        }

        [Test]
        public void SmsController_ExccedsTextMaxLimitOnLastSecondFromAccount_returnsMaxLimitFromAccountMessage() {
            // Arrange
            TextMessageDto textMessage = new TextMessageDto {
                Body = "Testing SMS 123",
                PhoneNumberFrom = "604-123-1234",
                PhoneNumberTo = "250-123-1234",
            };

            _mockTextMessageRepository.Setup(c => c.GetTextMessageCountPerAccountFromLastSecond()).Returns(100);
            _mockTextMessageRepository.Setup(c => c
                .GetTextMessageCountPerPhoneFromLastSecond(textMessage.PhoneNumberFrom))
                .Returns(4);

            // Act
            var result = _smsController.SendText(textMessage);
            var badRequestResult = result as BadRequestObjectResult;
            string? badRequestMessage = badRequestResult?.Value as string;

            // Assert
            Assert.That(badRequestResult?.StatusCode, Is.EqualTo(400));
            Assert.That(badRequestMessage, Is.EqualTo("Message Count exceeds maximum limit per Account from last second."));
        }

        [Test]
        public void SmsController_ExccedsTextMaxLimitOnLastSecondFromPhone_returnsMaxLimitFromPhoneMessage() {
            // Arrange
            TextMessageDto textMessage = new TextMessageDto {
                Body = "Testing SMS 123",
                PhoneNumberFrom = "604-123-1234",
                PhoneNumberTo = "250-123-1234",
            };

            _mockTextMessageRepository.Setup(c => c.GetTextMessageCountPerAccountFromLastSecond()).Returns(9);
            _mockTextMessageRepository.Setup(c => c
                .GetTextMessageCountPerPhoneFromLastSecond(textMessage.PhoneNumberFrom))
                .Returns(400);

            // Act
            var result = _smsController.SendText(textMessage);
            var badRequestResult = result as BadRequestObjectResult;
            string? badRequestMessage = badRequestResult?.Value as string;

            // Assert
            Assert.That(badRequestResult?.StatusCode, Is.EqualTo(400));
            Assert.That(badRequestMessage, Is.EqualTo("Message Count exceeds maximum limit per Phone from last second."));
        }

        [Test]
        public void SmsController_NotExccedsTextMaxLimitOnLastSecondFromPhoneAndAccount_returnsOk() {
            // Arrange
            TextMessageDto textMessage = new TextMessageDto {
                Body = "Testing SMS 123",
                PhoneNumberFrom = "604-123-1234",
                PhoneNumberTo = "250-123-1234",
            };

            _mockTextMessageRepository.Setup(c => c.GetTextMessageCountPerAccountFromLastSecond()).Returns(9);
            _mockTextMessageRepository.Setup(c => c
                .GetTextMessageCountPerPhoneFromLastSecond(textMessage.PhoneNumberFrom))
                .Returns(4);

            // Act
            var result = _smsController.SendText(textMessage);
            var okResult = result as OkObjectResult;
            string? okResultMessage = okResult?.Value as string;
k
            // Assert
            Assert.That(okResult?.StatusCode, Is.EqualTo(200));
            Assert.That(okResultMessage, Is.EqualTo("Success."));
        }

            // Arrange


            // Act


            // Assert
        
    }
}
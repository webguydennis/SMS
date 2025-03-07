using Moq;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using TextGateKeeper.Data;
using TextGateKeeper.Models;
using Microsoft.Extensions.Configuration;

namespace TextGateKeeper.Tests {
    
    [TestFixture]
    public class DataContextEFTests {
        private DataContextEF? _context;
        //private IConfiguration _config;
        private Mock<IConfiguration>? _mockConfiguration;

        [SetUp]
        public void SetUp()
        {
            
            var options = new DbContextOptionsBuilder<DataContextEF>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            
            _mockConfiguration = new Mock<IConfiguration>();
            Mock<IConfigurationSection> connectionStringSection = new Mock<IConfigurationSection>();
            //Mock<IConfigurationSection> maxLimitAccountSection = new Mock<IConfigurationSection>();
            //Mock<IConfigurationSection> maxLimitPhoneSection = new Mock<IConfigurationSection>();
            //Mock<IConfigurationSection> cutOffInDaysSection = new Mock<IConfigurationSection>();
            //"Server=TestServer;Database=TestDatabase;Trusted_Connection=True;"
            //"Server=localhost;Database=TextDatabase;TrustServerCertificate=true;User Id=test;Password=test;"
            
            connectionStringSection.Setup(s => s.Value).Returns(
                "Server=TestServer;Database=TestDatabase;Trusted_Connection=True;"
            );
            _mockConfiguration.Setup(c => c
                .GetSection("ConnectionStrings:DefaultConnection"))
                .Returns(connectionStringSection.Object);
            
            /*
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
            */
            
            _context = new DataContextEF(_mockConfiguration.Object);

            // Seed initial data 
            //if (_context.textMessages is not null) {
            _context.textMessages?.AddRange(
                new TextMessage { 
                    Id=1, 
                    Body="text 1", 
                    PhoneNumberFrom="123", 
                    PhoneNumberTo="321", 
                    CreatedDate=DateTime.Now 
                },
                new TextMessage { 
                    Id=2, 
                    Body="text 2", 
                    PhoneNumberFrom="456", 
                    PhoneNumberTo="654", 
                    CreatedDate=DateTime.Now 
                }
            );
            _context.SaveChanges();

            //}    
        }
        
        [TearDown]
        public void TearDown()
        {
            _ = (_context?.Database.EnsureDeleted());
            _context?.Dispose();
        }
        
        
        [Test]
        public void DataContextEF_ShouldRetrieveAllTextMessages_return2()
        {
            // Act
            if (_context?.textMessages is not null) {
                var result = _context.textMessages.ToList();

                // Assert
                Assert.That(result.Count, Is.EqualTo(2));
            }
        }
        
         
    }
}
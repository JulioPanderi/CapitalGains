using CapitalGains.Application.Interfaces;
using CapitalGains.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace CapitalGains.UnitTests.Console
{
    public class TestConsole
    {
        [Fact]
        public void Test_GetData()
        {
            //Mock the service provider
            var mockService = new Mock<IOperationsService>();

            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(sp => sp.GetService(typeof(IOperationsService)))
                               .Returns(mockService.Object);

            var mockServiceScope = new Mock<IServiceScope>();
            mockServiceScope.Setup(s => s.ServiceProvider)
                            .Returns(mockServiceProvider.Object);

            var mockServiceScopeFactory = new Mock<IServiceScopeFactory>();
            mockServiceScopeFactory.Setup(f => f.CreateScope())
                                   .Returns(mockServiceScope.Object);

            //Mock the logger
            var mock = new Mock<ILogger<OperationsService>>();
            ILogger<OperationsService> logger = mock.Object;

            //Arrange
            var operationsService = new OperationsService(mockServiceScopeFactory.Object, logger);
            string fileName = "DataFiles\\Case1.txt";

            //Act
            List<string> result = Program.GetData(fileName, operationsService);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.True(result.Any());
                Assert.True(result[0] == "[{\"tax\":0.0},{\"tax\":0.0},{\"tax\":0.0}]");
            });
        }

        [Fact]
        public void Test_GetData_Fail()
        {
            //Mock the service provider
            var mockService = new Mock<IOperationsService>();

            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(sp => sp.GetService(typeof(IOperationsService)))
                               .Returns(mockService.Object);

            var mockServiceScope = new Mock<IServiceScope>();
            mockServiceScope.Setup(s => s.ServiceProvider)
                            .Returns(mockServiceProvider.Object);

            var mockServiceScopeFactory = new Mock<IServiceScopeFactory>();
            mockServiceScopeFactory.Setup(f => f.CreateScope())
                                   .Returns(mockServiceScope.Object);

            //Mock the logger
            var mock = new Mock<ILogger<OperationsService>>();
            ILogger<OperationsService> logger = mock.Object;

            //Arrange
            var operationsService = new OperationsService(mockServiceScopeFactory.Object, logger);
            string fileName = "DataFiles\\Case.txt";

            //Act
            Action action = () => Program.GetData(fileName, operationsService);

            //Assert
            Assert.Throws<FileNotFoundException>(action);
        }
    }
}

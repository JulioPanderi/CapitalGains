using CapitalGains.Application.Interfaces;
using CapitalGains.Application.Services;
using CapitalGains.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace CapitalGains.UnitTests.Application
{
    public class TestOperationsService
    {
        private readonly decimal taxRate = 20;
        private readonly decimal minimumTaxed = 20000;

        #region DTOs for testing

        private readonly List<OperationDTO> testDTO_Case1 =
        [
            new OperationDTO() { Operation = "buy",  UnitCost = 10, Quantity = 100 },
            new OperationDTO() { Operation = "sell", UnitCost = 15, Quantity = 50 },
            new OperationDTO() { Operation = "sell", UnitCost = 15, Quantity = 50 }
        ];

        private readonly List<OperationDTO> testDTO_Case2 =
        [
            new OperationDTO() { Operation = "buy",  UnitCost = 10, Quantity = 10000 },
            new OperationDTO() { Operation = "sell", UnitCost = 20, Quantity = 5000 },
            new OperationDTO() { Operation = "sell", UnitCost = 5, Quantity = 5000 }
        ];

        private readonly List<OperationDTO> testDTO_Case3 =
        [
            new OperationDTO() { Operation = "buy",  UnitCost = 10, Quantity = 10000 },
            new OperationDTO() { Operation = "sell", UnitCost = 5, Quantity = 5000 },
            new OperationDTO() { Operation = "sell", UnitCost = 20, Quantity = 3000 }
        ];

        private readonly List<OperationDTO> testDTO_Case4 =
        [
            new OperationDTO() { Operation = "buy",  UnitCost = 10, Quantity = 10000 },
            new OperationDTO() { Operation = "buy", UnitCost = 25, Quantity = 5000 },
            new OperationDTO() { Operation = "sell", UnitCost = 15, Quantity = 10000 }
        ];

        private readonly List<OperationDTO> testDTO_Case5 =
        [
            new OperationDTO() { Operation = "buy",  UnitCost = 10, Quantity = 10000 },
            new OperationDTO() { Operation = "buy", UnitCost = 25, Quantity = 5000 },
            new OperationDTO() { Operation = "sell", UnitCost = 15, Quantity = 10000 },
            new OperationDTO() { Operation = "sell", UnitCost = 25, Quantity = 5000 }
        ];

        private readonly List<OperationDTO> testDTO_Case6 =
        [
            new OperationDTO() { Operation = "buy",  UnitCost = 10, Quantity = 10000 },
            new OperationDTO() { Operation = "sell", UnitCost = 2, Quantity = 5000 },
            new OperationDTO() { Operation = "sell", UnitCost = 20, Quantity = 2000 },
            new OperationDTO() { Operation = "sell", UnitCost = 20, Quantity = 2000 },
            new OperationDTO() { Operation = "sell", UnitCost = 25, Quantity = 1000 }
        ];

        private readonly List<OperationDTO> testDTO_Case7 =
        [
            new OperationDTO() { Operation = "buy",  UnitCost = 10, Quantity = 10000 },
            new OperationDTO() { Operation = "sell", UnitCost = 2, Quantity = 5000 },
            new OperationDTO() { Operation = "sell", UnitCost = 20, Quantity = 2000 },
            new OperationDTO() { Operation = "sell", UnitCost = 20, Quantity = 2000 },
            new OperationDTO() { Operation = "sell", UnitCost = 25, Quantity = 1000 },
            new OperationDTO() { Operation = "buy", UnitCost = 20, Quantity = 10000},
            new OperationDTO() { Operation = "sell", UnitCost = 15, Quantity = 5000 },
            new OperationDTO() { Operation = "sell", UnitCost = 30, Quantity = 4350 },
            new OperationDTO() { Operation = "sell", UnitCost = 30, Quantity = 650 }
        ];

        private readonly List<OperationDTO> testDTO_Case8 =
        [
            new OperationDTO() { Operation = "buy",  UnitCost = 10, Quantity = 10000 },
            new OperationDTO() { Operation = "sell", UnitCost = 50, Quantity = 10000 },
            new OperationDTO() { Operation = "buy", UnitCost = 20, Quantity = 10000 },
            new OperationDTO() { Operation = "sell", UnitCost = 50, Quantity = 10000 }
        ];

        private readonly List<OperationDTO> testDTO_Case9 =
        [
            new OperationDTO() { Operation = "buy",  UnitCost = 5000, Quantity = 10 },
            new OperationDTO() { Operation = "sell", UnitCost = 4000, Quantity = 5},
            new OperationDTO() { Operation = "buy", UnitCost = 15000, Quantity = 5},
            new OperationDTO() { Operation = "buy", UnitCost = 4000, Quantity = 2 },
            new OperationDTO() { Operation = "buy",  UnitCost = 23000, Quantity = 2 },
            new OperationDTO() { Operation = "sell", UnitCost = 20000, Quantity = 1 },
            new OperationDTO() { Operation = "sell", UnitCost =  12000, Quantity = 10 },
            new OperationDTO() { Operation = "sell", UnitCost = 15000, Quantity = 3 }
        ];

        private readonly List<OperationDTO> testDTO_Case_ArgumentException =
        [
            new OperationDTO() { Operation = "buy",  UnitCost = 10, Quantity = 100 },
            new OperationDTO() { Operation = "sel", UnitCost = 15, Quantity = 50 },
            new OperationDTO() { Operation = "sell", UnitCost = 15, Quantity = 50 }
        ];

        private readonly List<OperationDTO> testDTO_Case_ArgumentOutOfRangeException =
        [
            new OperationDTO() { Operation = "buy",  UnitCost = 10, Quantity = -100 },
            new OperationDTO() { Operation = "sell", UnitCost = 15, Quantity = 50 },
            new OperationDTO() { Operation = "sell", UnitCost = 15, Quantity = 50 }
        ];

        private readonly List<OperationDTO> testDTO_Case_InvalidOperationException =
        [
            new OperationDTO() { Operation = "buy",  UnitCost = 10, Quantity = 100 },
            new OperationDTO() { Operation = "sell", UnitCost = 15, Quantity = 50 },
            new OperationDTO() { Operation = "sell", UnitCost = 15, Quantity = 500 }
        ];

        #endregion

        [Fact]
        public void Test_ProcessMessage_Case1()
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

            //Act
            List<OperationDTO> testDTO = testDTO_Case1;
            List<ProcessedOperationDTO> result = operationsService.ProcessMessage(testDTO, taxRate, minimumTaxed);

            //Assert
            Assert.Multiple(() =>
            {
                //Same quantity of elements
                Assert.Equal(testDTO.Count, result.Count);
                //Verify total tax
                decimal totalTax = result.Sum(o => o.Tax);
                Assert.Equal(0, totalTax);
            });
        }

        [Fact]
        public void Test_ProcessMessage_Case2()
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

            //Act
            List<OperationDTO> testDTO = testDTO_Case2;
            List<ProcessedOperationDTO> result = operationsService.ProcessMessage(testDTO, taxRate, minimumTaxed);
            decimal totalTax = result.Sum(o => o.Tax);

            //Assert
            Assert.Multiple(() =>
            {
                //Same quantity of elements
                Assert.Equal(testDTO.Count, result.Count);
                //Verify total tax
                Assert.Equal(10000, totalTax);
                //Verify individual tax
                Assert.Equal(10000, result[1].Tax);
            });
        }

        [Fact]
        public void Test_ProcessMessage_Case3()
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

            //Act
            List<OperationDTO> testDTO = testDTO_Case3;
            List<ProcessedOperationDTO> result = operationsService.ProcessMessage(testDTO, taxRate, minimumTaxed);
            decimal totalTax = result.Sum(o => o.Tax);

            //Assert
            Assert.Multiple(() =>
            {
                //Same quantity of elements
                Assert.Equal(testDTO.Count, result.Count);
                //Verify total tax
                Assert.Equal(1000, totalTax);
                //Verify individual tax
                Assert.Equal(1000, result[2].Tax);
            });
        }

        [Fact]
        public void Test_ProcessMessage_Case4()
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

            //Act
            List<OperationDTO> testDTO = testDTO_Case4;
            List<ProcessedOperationDTO> result = operationsService.ProcessMessage(testDTO, taxRate, minimumTaxed);

            //Assert
            Assert.Multiple(() =>
            {
                //Same quantity of elements
                Assert.Equal(testDTO.Count, result.Count);
                //Verify total tax
                decimal totalTax = result.Sum(o => o.Tax);
                Assert.Equal(0, totalTax);
            });
        }

        [Fact]
        public void Test_ProcessMessage_Case5()
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

            //Act
            List<OperationDTO> testDTO = testDTO_Case5;
            List<ProcessedOperationDTO> result = operationsService.ProcessMessage(testDTO, taxRate, minimumTaxed);
            decimal totalTax = result.Sum(o => o.Tax);

            //Assert
            Assert.Multiple(() =>
            {
                //Same quantity of elements
                Assert.Equal(testDTO.Count, result.Count);
                //Verify total tax
                Assert.Equal(10000, totalTax);
                //Verify individual tax
                Assert.Equal(10000, result[3].Tax);
            });
        }

        [Fact]
        public void Test_ProcessMessage_Case6()
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

            //Act
            List<OperationDTO> testDTO = testDTO_Case6;
            List<ProcessedOperationDTO> result = operationsService.ProcessMessage(testDTO, taxRate, minimumTaxed);
            decimal totalTax = result.Sum(o => o.Tax);

            //Assert
            Assert.Multiple(() =>
            {
                //Same quantity of elements
                Assert.Equal(testDTO.Count, result.Count);
                //Verify total tax
                Assert.Equal(3000, totalTax);
                //Verify individual tax
                Assert.Equal(3000, result[4].Tax);
            });
        }

        [Fact]
        public void Test_ProcessMessage_Case7()
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

            //Act
            List<OperationDTO> testDTO = testDTO_Case7;
            List<ProcessedOperationDTO> result = operationsService.ProcessMessage(testDTO, taxRate, minimumTaxed);

            //Assert
            Assert.Multiple(() =>
            {
                //Same quantity of elements
                Assert.Equal(testDTO.Count, result.Count);
                //Verify total tax
                decimal totalTax = result.Sum(o => o.Tax);
                Assert.Equal(6700, totalTax);
                //Verify individual tax
                Assert.Equal(3000, result[4].Tax);
                Assert.Equal(3700, result[7].Tax);
            });
        }

        [Fact]
        public void Test_ProcessMessage_Case8()
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

            //Act
            List<OperationDTO> testDTO = testDTO_Case8;
            List<ProcessedOperationDTO> result = operationsService.ProcessMessage(testDTO, taxRate, minimumTaxed);
            decimal totalTax = result.Sum(o => o.Tax);

            //Assert
            Assert.Multiple(() =>
            {
                //Same quantity of elements
                Assert.Equal(testDTO.Count, result.Count);
                //Verify total tax
                Assert.Equal(140000, totalTax);
                //Verify individual tax
                Assert.Equal(80000, result[1].Tax);
                Assert.Equal(60000, result[3].Tax);
            });
        }

        [Fact]
        public void Test_ProcessMessage_Case9()
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

            //Act
            List<OperationDTO> testDTO = testDTO_Case9;
            List<ProcessedOperationDTO> result = operationsService.ProcessMessage(testDTO, taxRate, minimumTaxed);
            decimal totalTax = result.Sum(o => o.Tax);

            //Assert
            Assert.Multiple(() =>
            {
                //Same quantity of elements
                Assert.Equal(testDTO.Count, result.Count);
                //Verify total tax
                Assert.Equal(3400, totalTax);
                //Verify individual tax
                Assert.Equal(1000, result[6].Tax);
                Assert.Equal(2400, result[7].Tax);
            });
        }

        [Fact]
        public void Test_ProcessMessage_ArgumentException()
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

            //Act
            List<OperationDTO> testDTO = testDTO_Case_ArgumentException;

            //Act
            Action action = () => operationsService.ProcessMessage(testDTO, taxRate, minimumTaxed);

            //Assert
            Assert.Throws<ArgumentException>(action);
        }

        [Fact]
        public void Test_ProcessMessage_ArgumentOutOfRangeException()
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

            //Act
            List<OperationDTO> testDTO = testDTO_Case_ArgumentOutOfRangeException;

            //Act
            Action action = () => operationsService.ProcessMessage(testDTO, taxRate, minimumTaxed);

            //Assert
            Assert.Throws<ArgumentOutOfRangeException>(action);
        }

        [Fact]
        public void Test_ProcessMessage_InvalidOperationException()
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

            //Act
            List<OperationDTO> testDTO = testDTO_Case_InvalidOperationException;

            //Act
            Action action = () => operationsService.ProcessMessage(testDTO, taxRate, minimumTaxed);

            //Assert
            Assert.Throws<InvalidOperationException>(action);
        }
    }
}
using CapitalGains.Common;
using CapitalGains.Domain;
using System.Text.Json;

namespace CapitalGains.UnitTests.Common
{
    public class TestConverters
    {
        [Fact]
        public void Test_ConvertDTOtoJsonArray()
        {
            //Arrange
            List<ProcessedOperationDTO> testDTO =
            [
                new ProcessedOperationDTO(
                        new OperationDTO() {
                                Operation = "buy",
                                UnitCost = 10,
                                Quantity = 100 
                        })
                    {
                        Profit = 0,
                        Tax = 1.25m
                    }
            ];

            //Act
            string result = Converters.ConvertDTOtoJsonArray(testDTO);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.NotEmpty(result);
                Assert.Equal("[{\"tax\":1.25}]", result);
            });
            
        }

        [Fact]
        public void Test_ConvertJsonArrayToDTO_Ok()
        {
            //Arrange
            string jsonArray = "[{\"operation\":\"buy\", \"unit-cost\": 5000.00, \"quantity\": 10}]";

            //Act
            var result = Converters.ConvertJsonArrayToDTO(jsonArray);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.IsType<List<OperationDTO>>(result);
                Assert.NotNull(result);
                Assert.True(result.Any());
                Assert.True(result.Count==1);
                Assert.True(result[0].Operation == "buy");
            });

        }

        [Fact]
        public void Test_ConvertJsonArrayToDTO_ThrowsJsonException()
        {
            //Arrange
            string jsonArray = "[{\"operation\":\"buy\", \"unit-cost\": 5000.00, \"quantity\": 10}";

            //Act
            Action action = () => Converters.ConvertJsonArrayToDTO(jsonArray);

            //Assert
            Assert.Throws<JsonException>(action);
        }
    }
}

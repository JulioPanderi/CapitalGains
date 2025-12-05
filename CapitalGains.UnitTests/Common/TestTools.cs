using CapitalGains.Common;
using Moq;

namespace CapitalGains.UnitTests.Common
{
    public class TestTools
    {
        [Fact]
        public void Test_Read_File_OK()
        {
            //Act
            string text = Tools.ReadFile("DataFiles\\Case1.txt");

            //Assert
            Assert.NotEmpty(text);
        }

        [Fact]
        public void Test_Read_File_NotExists()
        {
            //Act
            string text = Tools.ReadFile("app.settings");

            //Assert
            Assert.Empty(text);
        }
    }
}

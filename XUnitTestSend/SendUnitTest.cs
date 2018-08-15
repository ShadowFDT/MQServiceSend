using System;
using Xunit;

namespace XUnitTestSend
{
    public class SendUnitTest
    {
        [Fact]
        public void ConsoleRead_NameSupplied()
        {
            //Arrange
            var rmq = new Send.RMQService();
            //Act
            string result = rmq.ConsoleRead("Luke");
            //Assert
            Assert.Equal("Hello my name is, Luke", result);
        }
    }
}

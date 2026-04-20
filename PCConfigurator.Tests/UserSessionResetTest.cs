using Xunit;

using PCConfigurator.UI.MVVM;
using PCConfigurator.UI.Services;


namespace PCConfigurator.Tests;


public class UserSessionResetTest
{
    [Fact]
    public void Reset_ClearsAllFields()
    {
        // Arrange
        var session = new UserSession
        {
            UserId = 42,
            Login = "john",
            Role = "admin"
        };

        // Act
        session.Reset();

        // Assert
        Assert.Equal(0, session.UserId);
        Assert.Equal("", session.Login);
        Assert.Equal("user", session.Role);
    }
}
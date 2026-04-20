using PCConfigurator.UI.MVVM;
using Xunit;

namespace PCConfigurator.Tests;

public class RelayCommandExecuteTest
{
    [Fact]
    public void Execute_InvokesAction()
    {
        // Arrange
        bool executed = false;
        var command = new RelayCommand(() => executed = true);

        // Act
        command.Execute(null);

        // Assert
        Assert.True(executed);
    }
}
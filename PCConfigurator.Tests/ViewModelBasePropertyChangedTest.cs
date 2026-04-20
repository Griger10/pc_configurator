using PCConfigurator.UI.MVVM;
using Xunit;

namespace PCConfigurator.Tests;


public class ViewModelBasePropertyChangedTest
{
    private class TestViewModel : ViewModelBase
    {
        private string _name = "";
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
    }

    [Fact]
    public void SetProperty_RaisesPropertyChanged_WithCorrectName()
    {
        // Arrange
        var vm = new TestViewModel();
        string? raisedProperty = null;
        vm.PropertyChanged += (_, e) => raisedProperty = e.PropertyName;

        // Act
        vm.Name = "TestValue";

        // Assert
        Assert.Equal("Name", raisedProperty);
    }
}
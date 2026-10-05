using LibrarySystem.Api.Validation;

namespace LibrarySystem.Api.Tests.Validation;

public class NotFutureYearAttributeTests
{
    [Fact]
    public void IsValid_WhenYearIsInFuture_ShouldReturnFalse()
    {
        // Arrange
        var attribute = new NotFutureYearAttribute();
        var futureYear = DateTime.UtcNow.Year + 1;

        // Act
        var result = attribute.IsValid(futureYear);

        // Assert
        Assert.False(result);
    }
}
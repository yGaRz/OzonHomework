using Ozon.Route256.Practice.OrdersService.Domain;

namespace TestServices.Domain;
public class CoordinateTests
{
    [Theory]
    [InlineData(100,-200)]
    [InlineData(91, 105)]
    [InlineData(-91, 100)]
    [InlineData(50, 181)]
    [InlineData(-90, -180.01)]
    [InlineData(-90.01, -200)]
    public void Create_Coordinates_Exception_Tests(double latitude, double longitude)
    {
        var act = () =>
        {
            var coordinate = Coordinates.CreateInstance(latitude, longitude);
        };
        Assert.Throws<DomainException>(act);
    }
    [Theory]
    [InlineData(90, 0)]
    [InlineData(-90, 10)]
    [InlineData(10, 180)]
    [InlineData(50, -180)]
    public void Create_Coordinates_Tests(double latitude, double longitude)
    {
        var coordinate = Coordinates.CreateInstance(latitude, longitude);
        Assert.NotNull(coordinate);
    }

    [Fact]
    public void Create_and_get_Coordinate_Test()
    {
        double latitude = 46.7;
        double longitude = -29.6;
        var coordinate =  Coordinates.CreateInstance(latitude, longitude);
        Assert.NotNull(coordinate);
        Assert.Equal(coordinate.Latitude, latitude);
        Assert.Equal(coordinate.Longitude, longitude);
    }

}

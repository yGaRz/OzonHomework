using Ozon.Route256.Practice.OrdersService.Domain;

namespace TestServices;
public class DomainTests
{
    [Fact]

    public void Create_Coordinates_Tests()
    {
        var act = () =>
        {
            var coordinate = Coordinates.CreateInstance(100, 10);
        };
        Assert.Throws<DomainException>(act);
    }
}

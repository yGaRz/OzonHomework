using Ozon.Route256.Practice.OrdersService.Domain;

namespace TestServices.Domain;

public class AddressTests
{
    [Theory]
    [InlineData(null,null,null,null,null,0,0)]
    [InlineData("aa", "bb", "cc", "dd", "ee", 100, 100)]
    [InlineData("", "bb", "cc", "dd", "ee", 50, 50)]
    [InlineData(null, "bb", "cc", "dd", "ee", 50, 50)]
    [InlineData("aa", "", "cc", "dd", "ee", 50, 50)]
    [InlineData("aa", null, "cc", "dd", "ee", 50, 50)]
    [InlineData("aa", "bb", "", "dd", "ee", 50, 50)]
    [InlineData("aa", "bb", null, "dd", "ee", 50, 50)]
    [InlineData("aa", "bb", "cc", "", "ee", 50, 50)]
    [InlineData("aa", "bb", "cc", null, "ee", 50, 50)]
    public void Create_Address_Exception_Test(string region,string city, string street, string building,string apartment, int latitude, int longitude)
    {
        var act = () => 
        {
            var address = Address.CreateInstance(region, city, street, building, apartment, Coordinates.CreateInstance(latitude, longitude));
        };
        Assert.Throws<DomainException>(act);
    }
    [Theory]
    [InlineData("aa", "bb", "cc", "dd", "ee", 50, 50)]
    [InlineData("aa", "bb", "cc", "dd", "", 50, 50)]
    [InlineData("aa", "bb", "cc", "dd", null, 50, 50)]
    public void Create_Address_No_Exception_Test(string region, string city, string street, string building, string apartment, int latitude, int longitude)
    {
        var address = Address.CreateInstance(region, city, street, building, apartment, Coordinates.CreateInstance(latitude, longitude));
        Assert.NotNull(address);
    }
    [Fact]
    public void Get_Data_From_Address_Test()
    {
        var address = Address.CreateInstance("aa", "bb", "cc", "dd", "ee", Coordinates.CreateInstance(50, 60));
        Assert.Equal("aa", address.Region);
        Assert.Equal("bb", address.City);
        Assert.Equal("cc", address.Street);
        Assert.Equal("dd", address.Building);
        Assert.Equal("ee", address.Apartment);
        Assert.Equal(50, address.Coordinates.Latitude);
        Assert.Equal(60, address.Coordinates.Longitude);
    }
    [Fact]
    public void Get_Data_From_Address_EmptyAparment_Test()
    {
        var address = Address.CreateInstance("aa", "bb", "cc", "dd", null, Coordinates.CreateInstance(50, 60));
        Assert.Equal("", address.Apartment);
    }
}

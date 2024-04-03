using Ozon.Route256.Practice.OrdersService.DataAccess;
using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;
namespace TestServices;
public class RegionRepositoryTest
{
    [Fact]
    public async Task TestCreateAsync1()
    {
        RegionRepository regionRepository = new RegionRepository();

        _ = regionRepository.CreateRegionAsync(new RegionEntity(0, "Moscow",1,1));
        var result = await regionRepository.GetNameByIdRegionAsync(0);    
        
        Assert.True(result == "Moscow");
    }

    [Fact]
    public async Task TestCreateAsync2()
    {
        RegionRepository regionRepository = new RegionRepository();

        _ = regionRepository.CreateRegionAsync(new RegionEntity(0, "Moscow", 1, 1));
        _ = regionRepository.CreateRegionAsync(new RegionEntity(0, "London", 1, 1));
        var result = await regionRepository.GetNameByIdRegionAsync(0);

        Assert.True(result != "London");
    }

    [Fact]
    public async Task TestCreateAsync3()
    {
        RegionRepository regionRepository = new RegionRepository();

        _ = regionRepository.CreateRegionAsync(new RegionEntity(0, "Moscow", 1, 1));
        _ = regionRepository.CreateRegionAsync(new RegionEntity(0, "London", 1, 1));
        _ = regionRepository.CreateRegionAsync(new RegionEntity(2, "London", 1, 1));
        _ = regionRepository.CreateRegionAsync(new RegionEntity(1, "Berlin", 1, 1));
        _ = regionRepository.CreateRegionAsync(new RegionEntity(6, "London", 1, 1));
        var result = await regionRepository.GetRegionsAsync();

        Assert.True(result.Count() == 3);
    }

    [Fact]
    public async Task TestCreateAsync4()
    {
        RegionRepository regionRepository = new RegionRepository();

        _ = regionRepository.CreateRegionAsync(new RegionEntity(0, "Moscow", 1, 1));
        _ = regionRepository.CreateRegionAsync(new RegionEntity(0, "London", 1, 1));
        _ = regionRepository.CreateRegionAsync(new RegionEntity(2, "London", 1, 1));
        _ = regionRepository.CreateRegionAsync(new RegionEntity(1, "Berlin", 1, 1));
        _ = regionRepository.CreateRegionAsync(new RegionEntity(6, "London", 1, 1));
        var result = await regionRepository.GetIdByRegionNameAsync("London");

        Assert.True(result == 2);
    }
}
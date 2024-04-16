namespace Ozon.Route256.Practice.OrdersService.Infrastructure.DAL.Models;

public class RegionDal
{
    public RegionDal(int id, string name, double latitude, double longitude)
    {
        Id = id;
        Name = name;
        Latitude = latitude;
        Longitude = longitude;
    }
    public RegionDal(int id, string name, decimal latitude, decimal longitude)
    {
        Id = id;
        Name = name;
        Latitude = (double)latitude;
        Longitude = (double)longitude;
    }

    public int Id {  get; set; }
    public string Name { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

using Ozon.Route256.Practice.OrdersService.Domain.Core;
using System.Dynamic;

namespace Ozon.Route256.Practice.OrdersService.Domain;

public sealed class Product:Entity<int>
{
    private Product(int id, string name, int quantity, double price, double weight):base(id)
    {
        Name = name;
        Quantity = quantity;
        Price = price;
        Weight = weight;
    }

    public static Product CreateInstance(int id, string name, int quantity, double price, double weight)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity <=0");
        if (price <= 0)
            throw new DomainException("Price <=0");
        if (weight <= 0)
            throw new DomainException("Weight <=0");
        return new Product(id, name, quantity, price, weight);
    }
    public string Name { get; }
    public int Quantity { get; }
    public double Price { get; }
    public double Weight { get; }
}



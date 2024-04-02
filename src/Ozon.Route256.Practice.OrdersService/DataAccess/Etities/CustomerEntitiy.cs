using System;
namespace Ozon.Route256.Practice.OrdersService.DataAccess.Etities;
//int32 id = 1;
//string first_name = 2;
//string last_name = 3;
//string mobile_number = 4;
//string email = 5;
//Address default_address = 6;
//repeated Address addressed = 7;
public record CustomerEntity
{
    public int Id { get; init; }
    public string FirstName { get; init; } = "";
    public string LastName { get; init; } = "";
    public string Phone { get; init; } = "";
    public string Email { get; init; } = "";
    public AddressEntity DefaultAddress { get; init; }=null;
    public AddressEntity[] Addressed { get; init; } = { };
    public static CustomerEntity ConvertFromCustomerGrpc(Customer customer)
    {
        return new CustomerEntity()
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Phone = customer.MobileNumber,
            Email = customer.Email,
            DefaultAddress = AddressEntity.ConvertFromAddressGrpc(customer.DefaultAddress),
            Addressed = customer.Addressed.Select(AddressEntity.ConvertFromAddressGrpc).ToArray()
        };
    }
    public static Customer ConvertToCustomerGrpc(CustomerEntity customer)
    {
        var result =  new Customer()
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            MobileNumber = customer.Phone,
            Email = customer.Email,
            DefaultAddress = AddressEntity.ConvertToAddressGrpc(customer.DefaultAddress)
        };
        result.Addressed.Add(customer.Addressed.Select(AddressEntity.ConvertToAddressGrpc));
        return result;
    }
}

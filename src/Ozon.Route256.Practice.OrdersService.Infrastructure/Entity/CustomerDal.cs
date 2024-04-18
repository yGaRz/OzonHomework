using Ozon.Route256.Practice.CustomerGprcFile;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Models;
internal record CustomerDal
{
    public int Id { get; init; }
    public string FirstName { get; init; } = "";
    public string LastName { get; init; } = "";
    public string Phone { get; init; } = "";
    public string Email { get; init; } = "";
    public AddressDal DefaultAddress { get; init; } = null;
    public AddressDal[] Addressed { get; init; } = { };
    public static CustomerDal ConvertFromCustomerGrpc(Customer customer)
    {
        return new CustomerDal()
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Phone = customer.MobileNumber,
            Email = customer.Email,
            DefaultAddress = AddressDal.ConvertFromAddressGrpc(customer.DefaultAddress),
            Addressed = customer.Addressed.Select(AddressDal.ConvertFromAddressGrpc).ToArray()
        };
    }
    public static Customer ConvertToCustomerGrpc(CustomerDal customer)
    {
        var result = new Customer()
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            MobileNumber = customer.Phone,
            Email = customer.Email,
            DefaultAddress = AddressDal.ConvertToAddressGrpc(customer.DefaultAddress)
        };
        result.Addressed.Add(customer.Addressed.Select(AddressDal.ConvertToAddressGrpc));
        return result;
    }
}

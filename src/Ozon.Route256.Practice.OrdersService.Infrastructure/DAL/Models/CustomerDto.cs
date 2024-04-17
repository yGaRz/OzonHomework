using Ozon.Route256.Practice.CustomerGprcFile;

namespace Ozon.Route256.Practice.OrdersService.DataAccess.Etities;
public record CustomerDto
{
    public int Id { get; init; }
    public string FirstName { get; init; } = "";
    public string LastName { get; init; } = "";
    public string Phone { get; init; } = "";
    public string Email { get; init; } = "";
    public AddressDto DefaultAddress { get; init; }=null;
    public AddressDto[] Addressed { get; init; } = { };
    public static CustomerDto ConvertFromCustomerGrpc(Customer customer)
    {
        return new CustomerDto()
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Phone = customer.MobileNumber,
            Email = customer.Email,
            DefaultAddress = AddressDto.ConvertFromAddressGrpc(customer.DefaultAddress),
            Addressed = customer.Addressed.Select(AddressDto.ConvertFromAddressGrpc).ToArray()
        };
    }
    public static Customer ConvertToCustomerGrpc(CustomerDto customer)
    {
        var result =  new Customer()
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            MobileNumber = customer.Phone,
            Email = customer.Email,
            DefaultAddress = AddressDto.ConvertToAddressGrpc(customer.DefaultAddress)
        };
        result.Addressed.Add(customer.Addressed.Select(AddressDto.ConvertToAddressGrpc));
        return result;
    }
}

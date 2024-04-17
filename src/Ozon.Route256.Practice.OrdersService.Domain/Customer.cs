using Ozon.Route256.Practice.OrdersService.Domain.Core;

namespace Ozon.Route256.Practice.OrdersService.Domain;

public sealed class Customer : Entity<int>
{
    private Customer(int id, string firstName, string lastName, string mobileNumber, Email email) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        MobileNumber = mobileNumber;
        Email = email;
    }
    public static Customer CreateInstance(int id, string firstName, string lastName, string mobileNumber, Email email)
    {
        if (firstName == null)
            throw new DomainException("Fisrt name is null");
        if (lastName == null)
            throw new DomainException("Last name is null");
        if (mobileNumber == null)
            throw new DomainException("Mobile number is null");
        return new Customer(id, firstName, lastName, mobileNumber, email);
    }
    public string FirstName { get; }
    public string LastName { get; }
    public string MobileNumber { get; }
    public Email Email { get; }
}

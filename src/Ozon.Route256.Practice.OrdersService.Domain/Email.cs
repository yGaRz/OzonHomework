using Ozon.Route256.Practice.OrdersService.Domain.Core;
using System.Text.RegularExpressions;

namespace Ozon.Route256.Practice.OrdersService.Domain;

public sealed class Email : ValueObject
{
    private Email(string value)
    {
        Value = value;
    }

    public static Email CreateInstance(string value)
    {
        if (value == null)
            throw new DomainException("Email is null");
        var regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        var match = regex.Match(value);
        if (!match.Success)
        {
            throw new DomainException($"Invalid email {value}");
        }
        return new Email(value);
    }

    public string Value { get; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

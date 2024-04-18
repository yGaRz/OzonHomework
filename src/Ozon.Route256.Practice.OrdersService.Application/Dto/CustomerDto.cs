using Ozon.Route256.Practice.OrdersService.Application.Dto;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Models;
//Чтобы можно было сгенерировать пользователей, по факту костыль для тестов.
public record CustomerDto
{
    public int Id { get; init; }
    public string FirstName { get; init; } = "";
    public string LastName { get; init; } = "";
    public string Phone { get; init; } = "";
    public string Email { get; init; } = "";
    public AddressDto DefaultAddress { get; init; } = null;
    public AddressDto[] Addressed { get; init; } = { };
}

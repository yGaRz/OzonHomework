using Ozon.Route256.Practice.OrdersService.Application.Dto;
using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Models;
namespace Ozon.Route256.Practice.OrdersService.DataAccess.Orders;

public interface IOrdersManager

{
    //Добавление заказа в репозиторий
    Task CreateOrderAsync(PreOrderDto order, CancellationToken token = default);

    //Выбор заказа по идентификатору
    Task<OrderDao> GetOrderByIdAsync(long id, CancellationToken token = default);

    //Смена состояния заказа
    Task<bool> SetOrderStateAsync(long id, OrderStateEnum state, DateTime timeUpdate, CancellationToken token = default);

    //выборка списка заказов, тут сделаем выборку по типу(web/api/site) + регион, сортировать будем выше.
    Task<OrderDao[]> GetOrdersByRegionAsync(List<string> regionList,
                                                        OrderSourceEnum source,
                                                        CancellationToken token = default);
    //Получение всех заказов клиента
    Task<OrderDao[]> GetOrdersByCutomerAsync(long idCustomer, DateTime dateStart, CancellationToken token = default);
    Task<RegionStatisticDto[]> GetRegionsStatisticAsync(List<string> regionList, DateTime dateStart, CancellationToken token = default);
}


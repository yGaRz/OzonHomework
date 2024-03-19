using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;
namespace Ozon.Route256.Practice.OrdersService.DataAccess
{
    public interface IOrdersRepository
    {
        //Добавление заказа в репозиторий
        Task CreateOrderAsync(OrderEntity order, CancellationToken token = default);
        //Выборка заказов по идентификатору
        Task<OrderEntity> GetOrderByCustomer(int id, CancellationToken token = default);
        //Смена состояния заказа
        Task<bool> SetOrderStateAsync(int id, OrderState state, CancellationToken token = default);
        //выборка списка заказов
        Task<List<OrderEntity>> GetOrdersByRegionAsync(List<string> regionList, 
                                                            OrderSource source, 
                                                            SortParam param=SortParam.None, 
                                                            string  field_name="", 
                                                            CancellationToken token = default);
        //Получение агегации(статистики) заказов по региону
        Task<List<RegionStatisticEntity>> GetRegionsStatisticAsync(List<string> regionList, DateTime dateStart, CancellationToken token = default);
        //Получение всех заказов клиента
        Task<List<OrderEntity>> GetOrdersByCutomer(ulong idCustomer, DateTime dateStart, CancellationToken token = default);
    }
}

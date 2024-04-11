using Bogus.DataSets;
using Ozon.Route256.Practice.OrdersService.Models;

namespace Ozon.Route256.Practice.OrdersService.DAL.Models;
public record OrderDal(
    long id,
    int customer_id,
    OrderSourceEnum source,
    OrderStateEnum state,
    DateTime timeCreate,
    DateTime timeUpdate,
    int regioId,
    int countGoods,
    double totalWeigth,
    double totalPrice,
    string addressJson);

//public class OrderDal
//{
//    public OrderDal(long id, int customer_id, OrderSourceEnum source, OrderStateEnum state, DateTime timeCreate,
//                    DateTime timeUpdate, int regioId, int countGoods, double totalWeigth, double totalPrice,
//                    string addressJson)
//    {
//        this.id = id;
//        this.customer_id = customer_id;
//        this.source = source;
//        this.state = state;
//        this.timeCreate = timeCreate;
//        this.timeUpdate = timeUpdate;
//        this.regioId = regioId;            
//        this.countGoods = countGoods;
//        this.totalWeigth = totalWeigth;
//        this.totalPrice = totalPrice;
//        this.addressJson = addressJson;

//    }
//    public long id {  get; set; }
//    public int customer_id {  get; set; }
//    public OrderSourceEnum source {  get; set; }
//    public OrderStateEnum state {  get; set; }
//    public DateTime timeCreate {  get; set; }
//    public DateTime timeUpdate {  get; set; }
//    public int regioId {  get; set; }
//    public int countGoods {  get; set; }
//    public double totalWeigth {  get; set; }
//    public double totalPrice {  get; set; }
//    public string addressJson {  get; set; }
//}


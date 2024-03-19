using System.ComponentModel.DataAnnotations;

namespace Ozon.Route256.Practice.GatewayService.Models
{
    public class GetOrdersModel
    {
        private List<string> _list = new List<string>();

        [Required]
        public List<string> RegionsList
        {
            get
            {
                return _list;
            }
            set
            {
                _list = value;
            }
        }
        [Required(ErrorMessage = "Не указан тип заказа")]
        [Range(0, 5, ErrorMessage = "Недопустимый тип заказа")]
        public OrderStateEnum State { get; set; }
        [Range(20, 50, ErrorMessage = "Превышен размер страницы[20,50]")]
        public uint PageSize { get; set; } = 25;
        [Range(0, 1)]
        public SortParam? SortParam { get; set; }
        public string? SortField { get; set; }
    }
}
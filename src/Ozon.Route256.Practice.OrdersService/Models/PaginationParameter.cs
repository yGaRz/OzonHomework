using System.ComponentModel.DataAnnotations;

namespace Ozon.Route256.Practice.OrdersService.Models
{
    public class PaginationParameter
    {
        //[Range(20, 50, ErrorMessage = "Превышен размер страницы")]
        //public uint MaxPageSize { get; set; } = 50;
        [Required]
        public uint PageIndex { get; set; }
        [Range(20, 50, ErrorMessage = "Превышен размер страницы[20,50]")]
        public uint PageSize { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Ozon.Route256.Practice.GatewayService.Models
{
    public class PaginationParameter
    {
        //private uint MaxPageSize { get; set; } = 50;
        [Required]
        public uint PageIndex { get; set; }
        [Range(20, 50, ErrorMessage = "Превышен размер страницы[20,50]")]
        public uint PageSize { get; set; } = 25;
    }
}

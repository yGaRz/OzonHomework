using System.ComponentModel.DataAnnotations;

namespace Ozon.Route256.Practice.GatewayService.Models
{
    public class GetOrdersModel : IValidatableObject
    {

        [Required]
        public List<string> RegionsList { get; set; } = new List<string>();

        [Required(ErrorMessage = "Не указан тип заказа")]
        [Range(0, 5, ErrorMessage = "Недопустимый тип заказа")]
        public OrderStateEnum State { get; set; }
        [Range(20, 50, ErrorMessage = "Превышен размер страницы[20,50]")]
        public uint PageSize { get; set; } = 25;
        [Range(0, 1)]
        public SortParam? SortParam { get; set; }
        public string? SortField { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();

            return errors;
        }
    }
}
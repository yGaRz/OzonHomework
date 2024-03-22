using System.ComponentModel.DataAnnotations;

namespace Ozon.Route256.Practice.GatewayService.Models
{

    public class GetOrdersModel : IValidatableObject
    {

        [Required]
        public List<string> RegionsList { get; set; } = new List<string>();

        [Required(ErrorMessage = "Не указан тип заказа")]
        [Range(1, 3, ErrorMessage = "Недопустимый тип заказа")]
        public OrderSourceEnum Source { get; set; }
        [Range(10, 100, ErrorMessage = "Превышен размер страницы[10,100]")]
        public uint PageSize { get; set; } = 25;
        [Range(0, 1)]
        public SortParamEnum? SParam { get; set; }
        public string? SortField { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();
            if (RegionsList.Count > 100)
                errors.Add(new ValidationResult("Превышен размер списка регионов(<100 названий)"));
            foreach (var region in RegionsList)
                if (region.Length < 3 || region.Length >= 50)
                {
                    errors.Add(new ValidationResult("Некорректная длинна названия региона(От 3 до 50 символов)"));
                    break;
                }


            return errors;
        }
    }
}
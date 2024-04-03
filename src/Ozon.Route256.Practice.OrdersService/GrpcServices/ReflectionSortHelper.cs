using System.Reflection;

namespace Ozon.Route256.Practice.OrdersService.GrpcServices
{
    public class ReflectionSortHelper
    {
        #pragma warning disable CS8602 // Разыменование вероятной пустой ссылки.
        #pragma warning disable CS8600 // Преобразование литерала, допускающего значение NULL или возможного значения NULL в тип, не допускающий значение NULL.
        private static MethodInfo GetCompareToMethod<T>(T genericInstance, string sortExpression)
        {
            Type genericType = genericInstance.GetType();

            object sortExpressionValue = genericType.GetProperty(sortExpression).GetValue(genericInstance, null);
            Type sortExpressionType = sortExpressionValue.GetType();
            MethodInfo compareToMethodOfSortExpressionType = sortExpressionType.GetMethod("CompareTo", new Type[] { sortExpressionType });
#pragma warning disable CS8603 // Возможно, возврат ссылки, допускающей значение NULL.
            return compareToMethodOfSortExpressionType;
#pragma warning restore CS8603 // Возможно, возврат ссылки, допускающей значение NULL.
        }
        public  static List<T> DynamicSort1<T>(List<T> genericList, string sortExpression, string sortDirection)
        {
            int sortReverser = sortDirection.ToLower().StartsWith("asc") ? 1 : -1;
            Comparison<T> comparisonDelegate = new Comparison<T>((x, y) =>
            {
                // Just to get the compare method info to compare the values.
                MethodInfo compareToMethod = GetCompareToMethod<T>(x, sortExpression);
                // Getting current object value.
                object xSortExpressionValue = x.GetType().GetProperty(sortExpression).GetValue(x, null);
                // Getting the previous value.
                object ySortExpressionValue = y.GetType().GetProperty(sortExpression).GetValue(y, null);
                // Comparing the current and next object value of collection.
#pragma warning disable CS8601 // Возможно, назначение-ссылка, допускающее значение NULL.
                object result = compareToMethod.Invoke(xSortExpressionValue, new object[] { ySortExpressionValue });
#pragma warning restore CS8601 // Возможно, назначение-ссылка, допускающее значение NULL.
                // Result tells whether the compared object is equal, greater, or lesser.
                return sortReverser * Convert.ToInt16(result);
            });
            // Using the comparison delegate to sort the object by its property.
            genericList.Sort(comparisonDelegate);

            return genericList;
        }
        #pragma warning restore CS8600 // Преобразование литерала, допускающего значение NULL или возможного значения NULL в тип, не допускающий значение NULL.
        #pragma warning restore CS8602 // Разыменование вероятной пустой ссылки.
    }
}

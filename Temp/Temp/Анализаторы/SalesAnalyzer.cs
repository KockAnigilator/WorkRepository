using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Temp.Модели;
using Temp.Сервисы;

namespace Temp.Анализаторы
{
    /// <summary>
    /// Класс для анализа данных о продажах
    /// </summary>
    public class SalesAnalyzer
    {
        /// <summary>
        /// Основной метод для удаления выбросов из данных
        /// </summary>
        public List<Sale> RemoveOutliers(List<Sale> sales)
        {
            if (sales == null || sales.Count == 0)
                return new List<Sale>();

            // 1. Группируем продажи по товарам
            var groupedSales = GroupSalesByProduct(sales);
            var filteredSales = new List<Sale>();

            // 2. Обрабатываем каждую группу товаров отдельно
            foreach (var productGroup in groupedSales)
            {
                // 3. Получаем статистику по продажам товара
                var stats = CalculateProductStatistics(productGroup.Value);

                // 4. Фильтруем данные по статистике
                FilterProductSales(productGroup.Value, stats, filteredSales);
            }

            return filteredSales;
        }

        /// <summary>
        /// Фильтрует выбросы и возвращает статистику по изменениям
        /// </summary>
        public (List<Sale> FilteredSales, Dictionary<string, (int Before, int After)> Stats)
            RemoveOutliersWithStats(List<Sale> sales)
        {
            var stats = new Dictionary<string, (int, int)>();
            var filtered = RemoveOutliers(sales);

            // Собираем статистику
            var groupedOriginal = GroupSalesByProduct(sales);
            var groupedFiltered = GroupSalesByProduct(filtered);

            foreach (var product in groupedOriginal.Keys)
            {
                stats[product] = (
                    groupedOriginal[product].Count,
                    groupedFiltered.ContainsKey(product) ? groupedFiltered[product].Count : 0
                );
            }

            return (filtered, stats);
        }

        /// <summary>
        /// Группирует продажи по названиям товаров
        /// </summary>
        private Dictionary<string, List<Sale>> GroupSalesByProduct(List<Sale> sales)
        {
            var groupedSales = new Dictionary<string, List<Sale>>();

            foreach (Sale sale in sales)
            {
                if (!groupedSales.ContainsKey(sale.Product))
                {
                    groupedSales[sale.Product] = new List<Sale>();
                }
                groupedSales[sale.Product].Add(sale);
            }

            return groupedSales;
        }

        /// <summary>
        /// Вычисляет статистические показатели для товара
        /// </summary>
        private (double median, double stdDev) CalculateProductStatistics(List<Sale> productSales)
        {
            var quantities = productSales.ConvertAll(sale => sale.Quantity);
            double median = StatisticsCalculator.CalculateMedian(quantities);
            double stdDev = StatisticsCalculator.CalculateStandardDeviation(quantities, median);

            return (median, stdDev);
        }

        /// <summary>
        /// Фильтрует продажи товара по статистическим границам
        /// </summary>
        private void FilterProductSales(List<Sale> productSales, (double median, double stdDev) stats,
                                      List<Sale> result)
        {
            double lowerBound = stats.median - 2 * stats.stdDev;
            double upperBound = stats.median + 2 * stats.stdDev;

            foreach (Sale sale in productSales)
            {
                if (sale.Quantity >= lowerBound && sale.Quantity <= upperBound)
                {
                    result.Add(sale);
                }
            }
        }

        /// <summary>
        /// Вычисляет средние продажи по всем товарам
        /// </summary>
        public Dictionary<string, double> CalculateAverageSales(List<Sale> sales)
        {
            var groupedSales = GroupSalesByProduct(sales);
            var averages = new Dictionary<string, double>();

            foreach (var productGroup in groupedSales)
            {
                double sum = 0;
                foreach (Sale sale in productGroup.Value)
                {
                    sum += sale.Quantity;
                }
                averages[productGroup.Key] = sum / productGroup.Value.Count;
            }

            return averages;
        }
    }
}

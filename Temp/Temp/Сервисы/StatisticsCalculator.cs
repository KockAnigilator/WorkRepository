using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Temp.Сервисы
{
    /// <summary>
    /// Класс для статистических вычислений
    /// </summary>
    public static class StatisticsCalculator
    {
        /// <summary>
        /// Вычисляет медиану значений
        /// </summary>
        public static double CalculateMedian(List<int> values)
        {
            if (values == null || values.Count == 0)
                throw new ArgumentException("Список значений не может быть пустым");

            var sortedValues = new List<int>(values);
            sortedValues.Sort();

            int count = sortedValues.Count;
            if (count % 2 == 0)
            {
                return (sortedValues[count / 2 - 1] + sortedValues[count / 2]) / 2.0;
            }
            return sortedValues[count / 2];
        }

        /// <summary>
        /// Вычисляет стандартное отклонение
        /// </summary>
        public static double CalculateStandardDeviation(List<int> values, double median)
        {
            if (values == null || values.Count == 0)
                throw new ArgumentException("Список значений не может быть пустым");

            double sumOfSquares = 0;
            foreach (int value in values)
            {
                sumOfSquares += Math.Pow(value - median, 2);
            }
            return Math.Sqrt(sumOfSquares / values.Count);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Temp.Сервисы
{
    /// <summary>
    /// Класс для прогнозирования спроса
    /// </summary>
    public class DemandForecaster
    {
        /// <summary>
        /// Рассчитывает прогноз спроса на указанное количество дней
        /// </summary>
        public Dictionary<string, double> ForecastDemand(Dictionary<string, double> averageSales, int days)
        {
            if (averageSales == null || days <= 0)
                return new Dictionary<string, double>();

            var forecast = new Dictionary<string, double>();

            foreach (var item in averageSales)
            {
                forecast[item.Key] = item.Value * days;
            }

            return forecast;
        }
    }
}

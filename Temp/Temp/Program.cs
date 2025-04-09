using System;
using System.Collections.Generic;
using System.IO;

namespace DemandForecastingApp
{
    /// <summary>
    /// Класс для хранения данных об одной продаже
    /// </summary>
    public class Sale
    {
        public string Product { get; set; }  // Название товара
        public DateTime Date { get; set; }   // Дата продажи
        public int Quantity { get; set; }    // Количество проданных единиц

        public Sale(string product, DateTime date, int quantity)
        {
            Product = product;
            Date = date;
            Quantity = quantity;
        }
    }

    /// <summary>
    /// Класс для анализа данных о продажах
    /// </summary>
    public class SalesAnalyzer
    {
        /// <summary>
        /// Удаляет аномальные значения (выбросы) из данных о продажах
        /// </summary>
        public List<Sale> RemoveOutliers(List<Sale> sales)
        {
            List<Sale> filteredSales = new List<Sale>();
            Dictionary<string, List<Sale>> groupedSales = new Dictionary<string, List<Sale>>();

            // Группируем продажи по товарам
            foreach (Sale sale in sales)
            {
                if (!groupedSales.ContainsKey(sale.Product))
                {
                    groupedSales[sale.Product] = new List<Sale>();
                }
                groupedSales[sale.Product].Add(sale);
            }

            // Обрабатываем каждую группу товаров
            foreach (var productGroup in groupedSales)
            {
                List<int> quantities = new List<int>();
                foreach (Sale sale in productGroup.Value)
                {
                    quantities.Add(sale.Quantity);
                }

                // Вычисляем медиану
                quantities.Sort();
                double median;
                int count = quantities.Count;

                if (count % 2 == 0)
                {
                    median = (quantities[count / 2 - 1] + quantities[count / 2]) / 2.0;
                }
                else
                {
                    median = quantities[count / 2];
                }

                // Вычисляем стандартное отклонение
                double sumOfSquares = 0;
                foreach (int quantity in quantities)
                {
                    sumOfSquares += Math.Pow(quantity - median, 2);
                }
                double stdDev = Math.Sqrt(sumOfSquares / quantities.Count);

                // Определяем границы нормальных значений
                double lowerBound = median - 2 * stdDev;
                double upperBound = median + 2 * stdDev;

                // Фильтруем продажи
                foreach (Sale sale in productGroup.Value)
                {
                    if (sale.Quantity >= lowerBound && sale.Quantity <= upperBound)
                    {
                        filteredSales.Add(sale);
                    }
                }
            }

            return filteredSales;
        }

        /// <summary>
        /// Вычисляет средние продажи по каждому товару
        /// </summary>
        public Dictionary<string, double> CalculateAverageSales(List<Sale> sales)
        {
            Dictionary<string, List<Sale>> groupedSales = new Dictionary<string, List<Sale>>();
            Dictionary<string, double> averageSales = new Dictionary<string, double>();

            // Группируем продажи по товарам
            foreach (Sale sale in sales)
            {
                if (!groupedSales.ContainsKey(sale.Product))
                {
                    groupedSales[sale.Product] = new List<Sale>();
                }
                groupedSales[sale.Product].Add(sale);
            }

            // Вычисляем средние значения
            foreach (var productGroup in groupedSales)
            {
                double sum = 0;
                foreach (Sale sale in productGroup.Value)
                {
                    sum += sale.Quantity;
                }
                averageSales[productGroup.Key] = sum / productGroup.Value.Count;
            }

            return averageSales;
        }
    }

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
            Dictionary<string, double> forecast = new Dictionary<string, double>();

            foreach (var item in averageSales)
            {
                forecast[item.Key] = item.Value * days;
            }

            return forecast;
        }
    }

    /// <summary>
    /// Класс для работы с файлами
    /// </summary>
    public class FileManager
    {
        /// <summary>
        /// Загружает данные о продажах из CSV-файла
        /// </summary>
        public List<Sale> LoadSalesFromCsv(string filePath)
        {
            List<Sale> sales = new List<Sale>();

            try
            {
                string[] lines = File.ReadAllLines(filePath);

                foreach (string line in lines)
                {
                    string[] parts = line.Split(';');

                    if (parts.Length == 3)
                    {
                        string product = parts[0].Trim();
                        DateTime date = DateTime.Parse(parts[1].Trim());
                        int quantity = int.Parse(parts[2].Trim());

                        sales.Add(new Sale(product, date, quantity));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении файла: {ex.Message}");
            }

            return sales;
        }

        /// <summary>
        /// Сохраняет прогноз в текстовый файл
        /// </summary>
        public void SaveForecastToFile(Dictionary<string, double> forecast, string filePath)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine("Прогноз спроса на товары:");
                    writer.WriteLine("--------------------------");

                    foreach (var item in forecast)
                    {
                        writer.WriteLine($"{item.Key}: {Math.Round(item.Value, 2)} единиц");
                    }

                    writer.WriteLine("--------------------------");
                    writer.WriteLine($"Дата создания: {DateTime.Now}");
                }

                Console.WriteLine($"Прогноз успешно сохранен в файл: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении файла: {ex.Message}");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Программа прогнозирования спроса");
            Console.WriteLine("---------------------------------");

            // Получаем путь к папке с исполняемым файлом
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

            // Получаем путь к папке проекта (на 3 уровня выше чем bin/Debug)
            string projectDir = Directory.GetParent(Directory.GetParent(Directory.GetParent(baseDir).FullName).FullName).FullName;

            // Формируем пути к файлам относительно папки проекта
            string salesFilePath = Path.Combine(projectDir, "sales.csv");
            string forecastFilePath = Path.Combine(projectDir, "forecast.txt");

            Console.WriteLine($"Ищем файл данных: {salesFilePath}");

            // Создаем экземпляр класса для работы с файлами
            FileManager fileManager = new FileManager();

            // Загружаем данные о продажах
            List<Sale> sales = fileManager.LoadSalesFromCsv(salesFilePath);

            if (sales.Count == 0)
            {
                Console.WriteLine("Нет данных для анализа. Проверьте файл sales.csv");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"Загружено {sales.Count} записей о продажах");

            // Создаем анализатор продаж
            SalesAnalyzer analyzer = new SalesAnalyzer();

            // Удаляем выбросы из данных
            Console.WriteLine("Фильтруем данные...");
            List<Sale> filteredSales = analyzer.RemoveOutliers(sales);
            Console.WriteLine($"После фильтрации осталось {filteredSales.Count} записей");

            // Вычисляем средние продажи по товарам
            Console.WriteLine("Вычисляем средние продажи...");
            Dictionary<string, double> averageSales = analyzer.CalculateAverageSales(filteredSales);

            // Создаем прогнозировщик спроса
            DemandForecaster forecaster = new DemandForecaster();

            // Запрашиваем у пользователя период прогнозирования
            Console.Write("\nВведите количество дней для прогноза: ");
            int forecastDays;
            while (!int.TryParse(Console.ReadLine(), out forecastDays) || forecastDays <= 0)
            {
                Console.Write("Введите положительное целое число: ");
            }

            // Получаем прогноз спроса
            Dictionary<string, double> forecast = forecaster.ForecastDemand(averageSales, forecastDays);

            // Выводим результаты
            Console.WriteLine("\nРезультаты прогноза:");
            Console.WriteLine("-------------------");
            foreach (var item in forecast)
            {
                Console.WriteLine($"{item.Key}: {Math.Round(item.Value, 2)} единиц");
            }
            Console.WriteLine("-------------------");

            // Сохраняем прогноз в файл
            fileManager.SaveForecastToFile(forecast, forecastFilePath);

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}
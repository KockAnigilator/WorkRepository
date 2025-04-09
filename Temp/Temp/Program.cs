using System;
using System.Collections.Generic;
using System.IO;

namespace DemandForecastingApp
{
    /// <summary>
    /// Класс для хранения данных о продажах товаров
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
        /// Основной метод для удаления выбросов из данных
        /// </summary>
        public List<Sale> RemoveOutliers(List<Sale> sales)
        {
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
            // 1. Получаем список количеств продаж
            var quantities = new List<int>();
            foreach (Sale sale in productSales)
            {
                quantities.Add(sale.Quantity);
            }

            // 2. Сортируем для вычисления медианы
            quantities.Sort();

            // 3. Вычисляем медиану
            double median = CalculateMedian(quantities);

            // 4. Вычисляем стандартное отклонение
            double stdDev = CalculateStandardDeviation(quantities, median);

            return (median, stdDev);
        }

        /// <summary>
        /// Вычисляет медиану значений
        /// </summary>
        private double CalculateMedian(List<int> values)
        {
            int count = values.Count;
            if (count % 2 == 0)
            {
                return (values[count / 2 - 1] + values[count / 2]) / 2.0;
            }
            return values[count / 2];
        }

        /// <summary>
        /// Вычисляет стандартное отклонение
        /// </summary>
        private double CalculateStandardDeviation(List<int> values, double median)
        {
            double sumOfSquares = 0;
            foreach (int value in values)
            {
                sumOfSquares += Math.Pow(value - median, 2);
            }
            return Math.Sqrt(sumOfSquares / values.Count);
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
            var forecast = new Dictionary<string, double>();

            foreach (var item in averageSales)
            {
                forecast[item.Key] = item.Value * days;
            }

            return forecast;
        }
    }

    /// <summary>
    /// Класс для работы с файлами данных
    /// </summary>
    public class FileManager
    {
        /// <summary>
        /// Загружает данные о продажах из CSV-файла
        /// </summary>
        public List<Sale> LoadSalesFromCsv(string filePath)
        {
            var sales = new List<Sale>();

            try
            {
                string[] lines = File.ReadAllLines(filePath);

                foreach (string line in lines)
                {
                    // Парсим строку формата: Товар;Дата;Количество
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
        /// Сохраняет результаты прогноза в текстовый файл
        /// </summary>
        public void SaveForecastToFile(Dictionary<string, double> forecast, string filePath)
        {
            try
            {
                using (var writer = new StreamWriter(filePath))
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

                Console.WriteLine($"Прогноз сохранен в файл: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении файла: {ex.Message}");
            }
        }
    }

    class Program
    {
        /// <summary>
        /// Основной метод программы
        /// </summary>
        static void Main(string[] args)
        {
            try
            {
                // 1. Инициализация и загрузка данных
                InitializeApplication();

                // 2. Получение и обработка данных
                var processedData = ProcessSalesData();

                // 3. Прогнозирование и вывод результатов
                GenerateAndDisplayForecast(processedData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("\nНажмите любую клавишу для выхода...");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Инициализирует приложение и загружает данные
        /// </summary>
        private static void InitializeApplication()
        {
            Console.WriteLine("Программа прогнозирования спроса");
            Console.WriteLine("---------------------------------");
        }

        /// <summary>
        /// Обрабатывает данные о продажах
        /// </summary>
        private static (List<Sale> filteredSales, Dictionary<string, double> averages) ProcessSalesData()
        {
            var fileManager = new FileManager();
            string filePath = GetDataFilePath();

            Console.WriteLine($"Загрузка данных из файла: {filePath}");
            var sales = fileManager.LoadSalesFromCsv(filePath);

            if (sales.Count == 0)
            {
                throw new Exception("Нет данных для анализа. Проверьте файл sales.csv");
            }

            Console.WriteLine($"Загружено {sales.Count} записей о продажах");

            var analyzer = new SalesAnalyzer();
            Console.WriteLine("Фильтрация данных...");
            var filteredSales = analyzer.RemoveOutliers(sales);
            Console.WriteLine($"После фильтрации осталось {filteredSales.Count} записей");

            Console.WriteLine("Вычисление средних продаж...");
            var averages = analyzer.CalculateAverageSales(filteredSales);

            return (filteredSales, averages);
        }

        /// <summary>
        /// Генерирует и отображает прогноз
        /// </summary>
        private static void GenerateAndDisplayForecast((List<Sale> filteredSales, Dictionary<string, double> averages) data)
        {
            var forecaster = new DemandForecaster();
            int days = GetForecastDays();

            Console.WriteLine("\nГенерация прогноза...");
            var forecast = forecaster.ForecastDemand(data.averages, days);

            DisplayForecastResults(forecast);
            SaveForecastResults(forecast);
        }

        /// <summary>
        /// Получает количество дней для прогноза от пользователя
        /// </summary>
        private static int GetForecastDays()
        {
            Console.Write("\nВведите количество дней для прогноза: ");
            int days;
            while (!int.TryParse(Console.ReadLine(), out days) || days <= 0)
            {
                Console.Write("Введите положительное целое число: ");
            }
            return days;
        }

        /// <summary>
        /// Отображает результаты прогноза
        /// </summary>
        private static void DisplayForecastResults(Dictionary<string, double> forecast)
        {
            Console.WriteLine("\nРезультаты прогноза:");
            Console.WriteLine("-------------------");
            foreach (var item in forecast)
            {
                Console.WriteLine($"{item.Key}: {Math.Round(item.Value, 2)} единиц");
            }
            Console.WriteLine("-------------------");
        }

        /// <summary>
        /// Сохраняет результаты прогноза в файл
        /// </summary>
        private static void SaveForecastResults(Dictionary<string, double> forecast)
        {
            var fileManager = new FileManager();
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "forecast.txt");
            fileManager.SaveForecastToFile(forecast, filePath);
        }

        /// <summary>
        /// Получает путь к файлу с данными
        /// </summary>
        private static string GetDataFilePath()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "sales.csv");
        }
    }
}
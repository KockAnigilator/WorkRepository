using System;
using System.Collections.Generic;
using System.IO;
using Temp.Пользовательский_интерфейс;
using Temp;
using Temp.Работа_с_файлами;
using Temp.Модели;
using Temp.Анализаторы;
using Temp.Сервисы;

namespace DemandForecastingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // 1. Инициализация приложения
                UserInterface.DisplayWelcomeMessage();

                // 2. Обработка данных
                var processedData = ProcessSalesData();

                // 3. Прогнозирование и вывод результатов
                GenerateAndDisplayForecast(processedData);
            }
            catch (Exception ex)
            {
                UserInterface.DisplayErrorMessage(ex.Message);
            }
            finally
            {
                UserInterface.WaitForUserInput();
            }
        }

        private static (List<Sale> filteredSales, Dictionary<string, double> averages) ProcessSalesData()
        {
            var fileManager = new FileManager();
            string filePath = GetDataFilePath();

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Файл данных не найден: {filePath}\n" +
                    "Пожалуйста, создайте файл sales.csv в директории приложения.");
            }

            Console.WriteLine($"Загрузка данных из файла: {filePath}");
            var sales = fileManager.LoadSalesFromCsv(filePath);

            if (sales.Count == 0)
            {
                throw new Exception("Файл не содержит данных для анализа. Проверьте содержимое файла sales.csv");
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

        private static void GenerateAndDisplayForecast((List<Sale> filteredSales, Dictionary<string, double> averages) data)
        {
            var forecaster = new DemandForecaster();
            int days = UserInterface.GetForecastDays();

            Console.WriteLine("\nГенерация прогноза...");
            var forecast = forecaster.ForecastDemand(data.averages, days);

            UserInterface.DisplayForecastResults(forecast);
            SaveForecastResults(forecast);
        }

        private static void SaveForecastResults(Dictionary<string, double> forecast)
        {
            try
            {
                var fileManager = new FileManager();
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "forecast.txt");
                fileManager.SaveForecastToFile(forecast, filePath);
                UserInterface.DisplayFileSavedMessage(filePath);
            }
            catch (Exception ex)
            {
                UserInterface.DisplayErrorMessage($"Не удалось сохранить прогноз: {ex.Message}");
            }
        }

        private static string GetDataFilePath()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "sales.csv");
        }
    }
}
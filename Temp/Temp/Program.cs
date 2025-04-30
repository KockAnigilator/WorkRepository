using System;
using System.Collections.Generic;
using System.IO;
using Temp.Пользовательский_интерфейс;
using Temp;
using Temp.Работа_с_файлами;
using Temp.Модели;
using Temp.Анализаторы;
using Temp.Сервисы;
using System.Security.Cryptography;
using System.Linq;

namespace DemandForecastingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                UserInterface.DisplayWelcomeMessage();

                // Загрузка и обработка данных
                var fileManager = new FileManager();
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "sales.csv");
                var sales = fileManager.LoadSalesFromCsv(filePath);

                var analyzer = new SalesAnalyzer();
                var (filteredSales, stats) = analyzer.RemoveOutliersWithStats(sales);
                UserInterface.DisplayFilterResults(stats);

                // Выбор товаров
                var averages = analyzer.CalculateAverageSales(filteredSales);
                var selectedProducts = UserInterface.GetProductsToForecast(averages);
                var filteredAverages = averages
                    .Where(x => selectedProducts.Contains(x.Key))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                // Прогнозирование
                var forecaster = new DemandForecaster();
                var days = UserInterface.GetForecastDays();
                var forecast = forecaster.ForecastDemand(filteredAverages, days);

                UserInterface.DisplayForecastResults(forecast);
                fileManager.SaveForecastToFile(forecast, "forecast.txt");
            }
            catch (Exception ex)
            {
                File.AppendAllText("errors.log", $"[{DateTime.Now:yyyy-MM-dd HH:mm}] {ex.Message}\n");
                UserInterface.DisplayErrorMessage(ex.Message);
            }
            finally
            {
                UserInterface.WaitForUserInput();
            }
        }
    }
}
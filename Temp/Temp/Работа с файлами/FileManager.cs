using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Temp.Модели;

namespace Temp.Работа_с_файлами
{
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

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Файл не найден: {filePath}");

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
                throw new InvalidOperationException($"Ошибка при чтении файла: {ex.Message}", ex);
            }

            return sales;
        }

        /// <summary>
        /// Сохраняет результаты прогноза в текстовый файл
        /// </summary>
        public void SaveForecastToFile(Dictionary<string, double> forecast, string filePath)
        {
            if (forecast == null || forecast.Count == 0)
                throw new ArgumentException("Нет данных для сохранения");

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
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Ошибка при сохранении файла: {ex.Message}", ex);
            }
        }
    }
}

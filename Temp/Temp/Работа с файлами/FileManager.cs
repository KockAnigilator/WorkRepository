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
            var errors = new List<string>();

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Файл не найден: {filePath}");

            try
            {
                string[] lines = File.ReadAllLines(filePath);
                for (int i = 0; i < lines.Length; i++)
                {
                    try
                    {
                        string[] parts = lines[i].Split(';');
                        if (parts.Length == 3)
                        {
                            sales.Add(new Sale(
                                parts[0].Trim(),
                                DateTime.Parse(parts[1].Trim()),
                                int.Parse(parts[2].Trim())
                            ));
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Строка {i + 1}: {lines[i]} | Ошибка: {ex.Message}");
                    }
                }

                if (errors.Any())
                {
                    File.WriteAllLines("format_errors.log", errors);
                    throw new InvalidDataException($"Обнаружено {errors.Count} ошибок формата. См. format_errors.log");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Ошибка чтения: {ex.Message}", ex);
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

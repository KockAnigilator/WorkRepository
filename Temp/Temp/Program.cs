using System;
using System.Collections.Generic;
using System.IO;

// Класс для хранения данных об одной продаже
class Sale
{
    // Название товара
    public string Product { get; set; }

    // Дата продажи
    public DateTime Date { get; set; }

    // Количество проданных единиц
    public int Quantity { get; set; }

    // Конструктор для создания объекта продажи
    public Sale(string product, DateTime date, int quantity)
    {
        Product = product;
        Date = date;
        Quantity = quantity;
    }
}

// Класс для анализа данных о продажах
class SalesAnalyzer
{
    // Метод для удаления аномальных значений (выбросов)
    public List<Sale> RemoveOutliers(List<Sale> sales)
    {
        // Создаем список для отфильтрованных продаж
        List<Sale> filteredSales = new List<Sale>();

        // Сначала группируем продажи по товарам
        Dictionary<string, List<Sale>> groupedSales = GroupSalesByProduct(sales);

        // Обрабатываем каждую группу товаров отдельно
        foreach (KeyValuePair<string, List<Sale>> productGroup in groupedSales)
        {
            // Получаем список количеств продаж для текущего товара
            List<int> quantities = new List<int>();
            foreach (Sale sale in productGroup.Value)
            {
                quantities.Add(sale.Quantity);
            }

            // Вычисляем медиану и стандартное отклонение
            double median = CalculateMedian(quantities);
            double stdDev = CalculateStandardDeviation(quantities, median);

            // Определяем границы нормальных значений
            double lowerBound = median - 2 * stdDev;
            double upperBound = median + 2 * stdDev;

            // Добавляем только те продажи, которые попадают в границы
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

    // Метод для группировки продаж по товарам
    private Dictionary<string, List<Sale>> GroupSalesByProduct(List<Sale> sales)
    {
        Dictionary<string, List<Sale>> groupedSales = new Dictionary<string, List<Sale>>();

        foreach (Sale sale in sales)
        {
            // Если товар уже есть в словаре, добавляем продажу в его список
            if (groupedSales.ContainsKey(sale.Product))
            {
                groupedSales[sale.Product].Add(sale);
            }
            // Если товара еще нет, создаем новую запись
            else
            {
                groupedSales[sale.Product] = new List<Sale> { sale };
            }
        }

        return groupedSales;
    }

    // Метод для вычисления медианы
    private double CalculateMedian(List<int> values)
    {
        // Создаем копию списка, чтобы не менять оригинал
        List<int> sortedValues = new List<int>(values);

        // Сортируем значения по возрастанию
        sortedValues.Sort();

        int count = sortedValues.Count;

        // Если количество элементов четное
        if (count % 2 == 0)
        {
            // Берем среднее двух центральных значений
            return (sortedValues[count / 2 - 1] + sortedValues[count / 2]) / 2.0;
        }
        // Если нечетное
        else
        {
            // Берем центральное значение
            return sortedValues[count / 2];
        }
    }

    // Метод для вычисления стандартного отклонения
    private double CalculateStandardDeviation(List<int> values, double mean)
    {
        double sumOfSquares = 0;

        // Считаем сумму квадратов отклонений от среднего
        foreach (int value in values)
        {
            sumOfSquares += Math.Pow(value - mean, 2);
        }

        // Делим сумму на количество элементов и извлекаем корень
        return Math.Sqrt(sumOfSquares / values.Count);
    }

    // Метод для вычисления средних продаж по товарам
    public Dictionary<string, double> CalculateAverageSales(List<Sale> sales)
    {
        Dictionary<string, List<Sale>> groupedSales = GroupSalesByProduct(sales);
        Dictionary<string, double> averageSales = new Dictionary<string, double>();

        foreach (KeyValuePair<string, List<Sale>> productGroup in groupedSales)
        {
            double sum = 0;

            // Считаем сумму продаж для текущего товара
            foreach (Sale sale in productGroup.Value)
            {
                sum += sale.Quantity;
            }

            // Вычисляем среднее значение
            double average = sum / productGroup.Value.Count;
            averageSales[productGroup.Key] = average;
        }

        return averageSales;
    }
}

// Класс для прогнозирования спроса
class DemandForecaster
{
    // Метод для создания прогноза спроса
    public Dictionary<string, double> ForecastDemand(
        Dictionary<string, double> averageSales,
        int forecastDays)
    {
        Dictionary<string, double> forecast = new Dictionary<string, double>();

        // Для каждого товара умножаем средние продажи на количество дней
        foreach (KeyValuePair<string, double> item in averageSales)
        {
            forecast[item.Key] = item.Value * forecastDays;
        }

        return forecast;
    }
}

// Класс для работы с файлами
class FileManager
{
    // Метод для загрузки данных из CSV-файла
    public List<Sale> LoadSalesFromCsv(string filePath)
    {
        List<Sale> sales = new List<Sale>();

        try
        {
            // Читаем все строки из файла
            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                // Разбиваем строку на части по разделителю ';'
                string[] parts = line.Split(';');

                // Проверяем, что строка содержит все необходимые данные
                if (parts.Length == 3)
                {
                    string product = parts[0].Trim();
                    DateTime date = DateTime.Parse(parts[1].Trim());
                    int quantity = int.Parse(parts[2].Trim());

                    // Создаем объект продажи и добавляем в список
                    sales.Add(new Sale(product, date, quantity));
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка при чтении файла: " + ex.Message);
        }

        return sales;
    }

    // Метод для сохранения прогноза в файл
    public void SaveForecastToFile(
        Dictionary<string, double> forecast,
        string filePath)
    {
        try
        {
            // Создаем поток для записи в файл
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Прогноз спроса на товары:");
                writer.WriteLine("--------------------------");

                // Записываем прогноз по каждому товару
                foreach (KeyValuePair<string, double> item in forecast)
                {
                    writer.WriteLine("{0}: {1} единиц",
                        item.Key,
                        Math.Round(item.Value, 2));
                }

                writer.WriteLine("--------------------------");
                writer.WriteLine("Дата создания: {0}", DateTime.Now);
            }

            Console.WriteLine("Прогноз успешно сохранен в файл: " + filePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка при сохранении файла: " + ex.Message);
        }
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("Программа прогнозирования спроса");
        Console.WriteLine("---------------------------------");

        // Создаем объект для работы с файлами
        FileManager fileManager = new FileManager();

        // Загружаем данные о продажах
        Console.WriteLine("Загружаем данные из файла sales.csv...");
        List<Sale> sales = fileManager.LoadSalesFromCsv("C:\\Users\\Савелий\\source\\repos\\WorkRepository\\Temp\\Temp\\sales.csv");

        // Проверяем, что данные загрузились
        if (sales.Count == 0)
        {
            Console.WriteLine("Нет данных для анализа. Проверьте файл sales.csv");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Загружено {0} записей о продажах", sales.Count);

        // Создаем анализатор продаж
        SalesAnalyzer analyzer = new SalesAnalyzer();

        // Удаляем выбросы из данных
        Console.WriteLine("Фильтруем данные...");
        List<Sale> filteredSales = analyzer.RemoveOutliers(sales);
        Console.WriteLine("После фильтрации осталось {0} записей", filteredSales.Count);

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
        foreach (KeyValuePair<string, double> item in forecast)
        {
            Console.WriteLine("{0}: {1} единиц", item.Key, Math.Round(item.Value, 2));
        }
        Console.WriteLine("-------------------");

        // Сохраняем прогноз в файлj
        fileManager.SaveForecastToFile(forecast, "C:\\Users\\Савелий\\source\\repos\\WorkRepository\\Temp\\Temp\\forecast.txt");

        Console.WriteLine("\nНажмите любую клавишу для выхода...");
        Console.ReadKey();
    }
}
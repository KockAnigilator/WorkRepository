using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Temp.Пользовательский_интерфейс
{
    /// <summary>
    /// Класс для взаимодействия с пользователем
    /// </summary>
    public static class UserInterface
    {
        /// <summary>
        /// Отображает приветственное сообщение
        /// </summary>
        public static void DisplayWelcomeMessage()
        {
            Console.WriteLine("Программа прогнозирования спроса");
            Console.WriteLine("---------------------------------");
        }

        /// <summary>
        /// Получает количество дней для прогноза от пользователя
        /// </summary>
        public static int GetForecastDays()
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
        public static void DisplayForecastResults(Dictionary<string, double> forecast)
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
        /// Отображает сообщение о сохранении файла
        /// </summary>
        public static void DisplayFileSavedMessage(string filePath)
        {
            Console.WriteLine($"Прогноз сохранен в файл: {filePath}");
        }

        /// <summary>
        /// Отображает сообщение об ошибке
        /// </summary>
        public static void DisplayErrorMessage(string message)
        {
            Console.WriteLine($"Ошибка: {message}");
        }

        /// <summary>
        /// Ожидает нажатия клавиши пользователем
        /// </summary>
        public static void WaitForUserInput()
        {
            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}

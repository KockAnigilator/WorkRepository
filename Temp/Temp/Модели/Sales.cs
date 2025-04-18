using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Temp.Модели
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
}

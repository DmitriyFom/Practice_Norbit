using System;

namespace ProductApp
{
    /// <summary>
    /// Класс продукта.
    /// </summary>
    public class Product
    {
        private string _name = string.Empty;
        private string _manufacturer = string.Empty;
        private decimal _price;
        private int _shelfLifeDays;
        private DateTime _productionDate;

        /// <summary>
        /// Наименование товара.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Наименование не может быть пустым.", nameof(value));
                _name = value;
            }
        }

        /// <summary>
        /// Производитель.
        /// </summary>
        public string Manufacturer
        {
            get => _manufacturer;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Производитель не может быть пустым.", nameof(value));
                _manufacturer = value;
            }
        }

        /// <summary>
        /// Цена товара.
        /// </summary>
        public decimal Price
        {
            get => _price;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Цена не может быть отрицательной.", nameof(value));
                _price = value;
            }
        }

        /// <summary>
        /// Срок годности.
        /// </summary>
        public int ShelfLifeDays
        {
            get => _shelfLifeDays;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Срок годности должен быть положительным числом.", nameof(value));
                _shelfLifeDays = value;
            }
        }

        /// <summary>
        /// Дата производства.
        /// </summary>
        public DateTime ProductionDate
        {
            get => _productionDate;
            set
            {
                if (value > DateTime.Now)
                    throw new ArgumentException("Дата производства не может быть в будущем.", nameof(value));
                _productionDate = value;
            }
        }

        /// <summary>
        /// Вычисление даты срока годности.
        /// </summary>
        public DateTime ExpirationDate => _productionDate.AddDays(_shelfLifeDays);

   
        public Product(string name, string manufacturer, decimal price, int shelfLifeDays, DateTime productionDate)
        {
            ArgumentNullException.ThrowIfNull(name);
            ArgumentNullException.ThrowIfNull(manufacturer);

            Name = name;
            Manufacturer = manufacturer;
            Price = price;
            ShelfLifeDays = shelfLifeDays;
            ProductionDate = productionDate;
        }

        /// <summary>
        /// Переопределение метода ToString.
        /// </summary>
        public override string ToString()
        {
            return $"=== Товар ===\n" +
                   $"Наименование:     {_name}\n" +
                   $"Производитель:    {_manufacturer}\n" +
                   $"Цена:             {_price:F2} руб.\n" +
                   $"Дата производства: {_productionDate:dd.MM.yyyy}\n" +
                   $"Срок годности:    {_shelfLifeDays} дн.\n" +
                   $"Годен до:         {ExpirationDate:dd.MM.yyyy}";
        }

        public static Product CreateFromConsole()
        {
            Console.WriteLine("Введите информацию о товаре");

            Console.Write("Наименование: ");
            string name = Console.ReadLine() ?? string.Empty;

            Console.Write("Производитель: ");
            string manufacturer = Console.ReadLine() ?? string.Empty;

            Console.Write("Цена (руб.): ");
            decimal price = ReadDecimal();

            Console.Write("Срок годности (дней): ");
            int shelfLife = ReadInt();

            Console.Write("Дата производства (дд.мм.гггг) или Enter для текущей: ");
            DateTime productionDate = ReadDate();

            return new Product(name, manufacturer, price, shelfLife, productionDate);
        }

        private static decimal ReadDecimal()
        {
            while (true)
            {
                string? input = Console.ReadLine();
                decimal result; 
                if (decimal.TryParse(input, out result) && result >= 0)
                    return result;
                Console.Write("Некорректный ввод. Повторите: ");
            }
        }

        private static int ReadInt()
        {
            while (true)
            {
                string? input = Console.ReadLine();
                int result; 
                if (int.TryParse(input, out result) && result > 0)
                    return result;
                Console.Write("Некорректный ввод. Повторите: ");
            }
        }

        private static DateTime ReadDate()
        {
            string? input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
                return DateTime.Now;

            DateTime result; 
            while (!DateTime.TryParseExact(input, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out result))
            {
                Console.Write("Неверный формат даты. Повторите (дд.мм.гггг): ");
                input = Console.ReadLine();
            }
            return result;
        }
    }

    /// <summary>
    /// Точка входа.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Product product = Product.CreateFromConsole();
                Console.WriteLine();
                Console.WriteLine(product);
                Console.WriteLine("\nизменение свойств");
                product.Price = product.Price * 1.1m;
                Console.WriteLine($"Новая цена после повышения на 10%: {product.Price:F2} руб.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nОшибка: {ex.Message}");
            }

            Console.ReadLine();
        }
    }
}
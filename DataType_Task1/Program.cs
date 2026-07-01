using System;
using System.Text;

namespace FinanceApp
{
    /// <summary>
    /// Класс для финансовых расчетов.
    /// </summary>
    public class FinanceCalculator
    {
        /// <summary>
        /// Формирует строку с расчетом сложных процентов по годам.
        /// </summary>
        public static string CalculateCompoundInterest(decimal initialDeposit, int years, decimal interestRate)
        {
            if (initialDeposit <= 0 || years <= 0 || interestRate < 0)
            {
                throw new ArgumentException("Параметры должны быть положительными числами.");
            }

            var resultBuilder = new StringBuilder();
            decimal currentAmount = initialDeposit;
            decimal rateMultiplier = 1 + (interestRate / 100);

            for (int i = 1; i <= years; i++)
            {
                currentAmount *= rateMultiplier;
                resultBuilder.AppendLine($"Год {i}: {currentAmount:F2} руб.");
            }

            return resultBuilder.ToString().TrimEnd();
        }
    }

    /// <summary>
    /// Точка входа в программу.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {

            try
            {
                decimal myDeposit = ReadDecimal("Введите начальный вклад (руб.): ");
                int myYears = ReadInt("Введите количество лет: ");
                decimal myRate = ReadDecimal("Введите годовую процентную ставку (%): ");
                Console.WriteLine();

                string report = FinanceCalculator.CalculateCompoundInterest(myDeposit, myYears, myRate);

                Console.WriteLine("Отчет по вкладу");
                Console.WriteLine(report);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Ошибка: {ex.Message}");
            }

            Console.WriteLine("\nНажмите Enter для выхода...");
            Console.ReadLine();
        }

        private static decimal ReadDecimal(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();

                // Пытаемся преобразовать строку в decimal
                if (decimal.TryParse(input, out decimal result))
                {
                    return result;
                }

                Console.WriteLine("Некорректный ввод. Пожалуйста, введите число.\n");
            }
        }
        private static int ReadInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();
                if (int.TryParse(input, out int result))
                {
                    return result;
                }

                Console.WriteLine("Некорректный ввод. Пожалуйста, введите целое число.\n");
            }
        }
    }
}
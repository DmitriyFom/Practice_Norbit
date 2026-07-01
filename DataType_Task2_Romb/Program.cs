using System;
using System.Text;

namespace DiamondApp
{
    public class DiamondPrinter
    {
        public static void PrintDiamond(int n)
        {
            if (n <= 0 || n % 2 == 0)
            {
                throw new ArgumentException("Значение N должно быть положительным нечётным числом", nameof(n));
            }

            var result = new StringBuilder();
            int center = n / 2;

            for (int i = 0; i < n; i++)
            {
                int distance = Math.Abs(i - center);
                int leftPos = distance;
                int rightPos = n - 1 - distance;

                var line = new StringBuilder();

                for (int j = 0; j < n; j++)
                {
                    if (j == leftPos || j == rightPos)
                    {
                        line.Append('X');
                    }
                    else
                    {
                        line.Append(' ');
                    }
                }

                result.AppendLine(line.ToString());
            }

            Console.Write(result.ToString());
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                DiamondPrinter.PrintDiamond(5);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            Console.ReadLine();
        }
    }
}
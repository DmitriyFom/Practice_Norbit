using System;
using System.Collections;
using System.Collections.Generic;

namespace SmartStackApp
{
    /// <summary>
    /// стек на базе массива
    /// </summary>
    /// <typeparam name="T">тип элементов стека.</typeparam>
    public class SmartStack<T> : IEnumerable<T>, IEnumerable
    {
        private T[] _items;
        private int _count;

        public SmartStack()
        {
            _items = new T[4];
            _count = 0;
        }

        public SmartStack(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), "ёмкость не может быть отрицательной.");
            }
            _items = new T[capacity];
            _count = 0;
        }

        public SmartStack(IEnumerable<T> collection)
        {
            ArgumentNullException.ThrowIfNull(collection);

            int count = 0;
            foreach (var item in collection)
            {
                count++;
            }

            _items = new T[count];
            _count = 0;

            int index = count - 1;
            foreach (var item in collection)
            {
                _items[index] = item;
                index--;
            }
            _count = count;
        }

        public int Count => _count;
        public int Capacity => _items.Length;

        public T this[int depth]
        {
            get
            {
                if (depth < 0 || depth >= _count)
                {
                    throw new ArgumentOutOfRangeException(nameof(depth), "индекс выходит за границы стека.");
                }
                return _items[_count - 1 - depth];
            }
        }

        public void Push(T item)
        {
            if (_count == _items.Length)
            {
                ResizeArray(_items.Length * 2);
            }
            _items[_count] = item;
            _count++;
        }

        public void PushRange(IEnumerable<T> items)
        {
            ArgumentNullException.ThrowIfNull(items);

            int itemsCount = 0;
            foreach (var item in items)
            {
                itemsCount++;
            }

            while (_count + itemsCount > _items.Length)
            {
                ResizeArray(_items.Length * 2);
            }

            int index = _count + itemsCount - 1;
            foreach (var item in items)
            {
                _items[index] = item;
                index--;
            }

            _count += itemsCount;
        }

        public T Pop()
        {
            if (_count == 0)
            {
                throw new InvalidOperationException("Стек пуст.");
            }
            _count--;
            T item = _items[_count];
            _items[_count] = default!;
            return item;
        }

        public T Peek()
        {
            if (_count == 0)
            {
                throw new InvalidOperationException("стек пуст.");
            }
            return _items[_count - 1];
        }

        public bool Contains(T item)
        {
            var comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < _count; i++)
            {
                if (comparer.Equals(_items[i], item))
                {
                    return true;
                }
            }
            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = _count - 1; i >= 0; i--)
            {
                yield return _items[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void ResizeArray(int newCapacity)
        {
            T[] newArray = new T[newCapacity];
            Array.Copy(_items, 0, newArray, 0, _count);
            _items = newArray;
        }
    }

    /// <summary>
    /// Точка входа.
    /// </summary>
    class Program
    {
        private static SmartStack<int>? _stack;

        static void Main(string[] args)
        {
            bool exit = false;
            while (!exit)
            {
                PrintMenu();
                string? choice = Console.ReadLine()?.Trim();

                Console.WriteLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            CreateStack();
                            break;
                        case "2":
                            PushElement();
                            break;
                        case "3":
                            PushRangeElements();
                            break;
                        case "4":
                            PopElement();
                            break;
                        case "5":
                            PeekElement();
                            break;
                        case "6":
                            CheckContains();
                            break;
                        case "7":
                            ShowStackInfo();
                            break;
                        case "8":
                            AccessByIndex();
                            break;
                        case "9":
                            ShowStackContents();
                            break;
                        case "0":
                            exit = true;
                            Console.WriteLine("завершение");
                            break;
                        default:
                            Console.WriteLine("неизвестная команда\n");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nошибка: {ex.Message}\n");
                }
            }

            Console.ReadLine();
        }

        private static void PrintMenu()
        {
            Console.WriteLine("1. создать новый стек");
            Console.WriteLine("2. push — добавить элемент");
            Console.WriteLine("3. pushRange — добавить несколько элементов");
            Console.WriteLine("4. pop — извлечь элемент с вершины");
            Console.WriteLine("5. peek — посмотреть вершину");
            Console.WriteLine("6. проверить наличие элемента");
            Console.WriteLine("7. показать Count и Capacity");
            Console.WriteLine("8. доступ по индексу (глубине)");
            Console.WriteLine("9. показать весь стек ");
            Console.WriteLine("0. выход");
            Console.Write("выберите пункт меню: ");
        }

        private static void CreateStack()
        {
            Console.WriteLine("создание нового стека");
            Console.WriteLine("выберите способ создания:");
            Console.WriteLine("1. пустой стек");
            Console.WriteLine("2. с указанной ёмкостью");
            Console.WriteLine("3. из коллекции (введите числа через пробел)");

            string? choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    _stack = new SmartStack<int>();
                    Console.WriteLine("создан пустой стек ёмкостью 4.\n");
                    break;

                case "2":
                    Console.Write("сведите начальную ёмкость: ");
                    int capacity = int.Parse(Console.ReadLine()!);
                    _stack = new SmartStack<int>(capacity);
                    Console.WriteLine($"создан стек ёмкостью {capacity}.\n");
                    break;

                case "3":
                    Console.Write("введите числа через пробел (например: 10 20 30 40 50): ");
                    string? input = Console.ReadLine();
                    var numbers = new List<int>();
                    if (!string.IsNullOrWhiteSpace(input))
                    {
                        var parts = input.Split(new[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var part in parts)
                        {
                            numbers.Add(int.Parse(part));
                        }
                    }
                    _stack = new SmartStack<int>(numbers);
                    Console.WriteLine($"создан стек из {numbers.Count} элементов.\n");
                    ShowStackContents();
                    break;

                default:
                    Console.WriteLine("неверный выбор. стек не создан.\n");
                    break;
            }
        }

        private static void PushElement()
        {
            Console.Write("введите число для добавления: ");
            int value = int.Parse(Console.ReadLine()!);
            _stack!.Push(value);
            Console.WriteLine($"элемент {value} добавлен на вершину стека.\n");
        }

        private static void PushRangeElements()
        {
            Console.Write("введите числа через пробел для добавления: ");
            string? input = Console.ReadLine();
            var numbers = new List<int>();
            if (!string.IsNullOrWhiteSpace(input))
            {
                var parts = input.Split(new[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in parts)
                {
                    numbers.Add(int.Parse(part));
                }
            }

            if (numbers.Count == 0)
            {
                Console.WriteLine("не введено ни одного числа.\n");
                return;
            }

            _stack!.PushRange(numbers);
            Console.WriteLine($"добавлено {numbers.Count} элементов.\n");
            ShowStackContents();
        }

        /// <summary>
        /// извлекает элемент с вершины стека.
        /// </summary>
        private static void PopElement()
        {
            int value = _stack!.Pop();
            Console.WriteLine($"извлечён элемент с вершины: {value}\n");
        }

        /// <summary>
        /// показывает элемент на вершине стека.
        /// </summary>
        private static void PeekElement()
        {
            int value = _stack!.Peek();
            Console.WriteLine($"элемент на вершине стека: {value}\n");
        }

        /// <summary>
        /// проверяет наличие элемента в стеке.
        /// </summary>
        private static void CheckContains()
        {
            Console.Write("введите число для поиска: ");
            int value = int.Parse(Console.ReadLine()!);
            bool found = _stack!.Contains(value);
            Console.WriteLine(found
                ? $"элемент {value} НАЙДЕН в стеке.\n"
                : $"элемент {value} НЕ найден в стеке.\n");
        }

        /// <summary>
        /// показывает количество элементов и ёмкость стека.
        /// </summary>
        private static void ShowStackInfo()
        {
            Console.WriteLine($"Count (элементов):  {_stack!.Count,-12}");
            Console.WriteLine($"Capacity (ёмкость): {_stack!.Capacity,-12}");
        }

        /// <summary>
        /// доступ к элементу по глубине.
        /// </summary>
        private static void AccessByIndex()
        {
            Console.Write($"введите глубину (0 — вершина, {_stack!.Count - 1} — основание): ");
            int depth = int.Parse(Console.ReadLine()!);
            int value = _stack![depth];
            Console.WriteLine($"элемент на глубине {depth}: {value}\n");
        }

        /// <summary>
        /// выводит весь стек через foreach.
        /// </summary>
        private static void ShowStackContents()
        {
            Console.Write("содержимое стека (от вершины к основанию): ");
            if (_stack!.Count == 0)
            {
                Console.WriteLine("(стек пуст)");
            }
            else
            {
                foreach (var item in _stack)
                {
                    Console.Write($"{item} ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}
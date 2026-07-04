using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using AutoSalonCrud.Data;
using AutoSalonCrud.Entities;
using AutoSalonCrud.Repositories;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var connectionString = configuration.GetConnectionString("AutoSalonDb")
    ?? throw new InvalidOperationException("Строка подключения не найдена!");

bool exit = false;
while (!exit)
{
    Console.Clear();
    Console.WriteLine("Выберите способ доступа к данным:");
    Console.WriteLine("1. ADO.NET");
    Console.WriteLine("2. Entity Framework");
    Console.WriteLine("0. Выход");
    Console.Write("\nВаш выбор: ");

    string? choice = Console.ReadLine()?.Trim();
    ICarRepository? repository = null;
    string techName = "";

    switch (choice)
    {
        case "1":
            repository = new AdoNetCarRepository(connectionString);
            techName = "ADO.NET";
            break;
        case "2":
            var optionsBuilder = new DbContextOptionsBuilder<AutoSalonContext>();
            optionsBuilder.UseSqlServer(connectionString);
            var context = new AutoSalonContext(optionsBuilder.Options);
            repository = new EfCoreCarRepository(context);
            techName = "EF Core";
            break;
        case "0":
            exit = true;
            Console.WriteLine("Завершение работы!");
            continue;
        default:
            Console.WriteLine("Неверный выбор. Попробуйте снова.");
            Console.ReadKey();
            continue;
    }
    bool backToMain = false;
    while (!backToMain && repository != null)
    {
        Console.Clear();
        Console.WriteLine($"Режим работы: {techName,-35}");
        Console.WriteLine("1. Показать все автомобили");
        Console.WriteLine("2. Добавить новый автомобиль");
        Console.WriteLine("3  Изменить автомобиль");
        Console.WriteLine("4. Удалить автомобиль");
        Console.WriteLine("0. Вернуться в главное меню");
        Console.Write("\nВаш выбор: ");

        string? action = Console.ReadLine()?.Trim();
        Console.WriteLine();

        try
        {
            switch (action)
            {
                case "1":
                    await ShowAllCarsAsync(repository);
                    break;
                case "2":
                    await AddNewCarAsync(repository, connectionString);
                    break;
                case "3":
                    await UpdateCarAsync(repository);
                    break;
                case "4":
                    await DeleteCarAsync(repository);
                    break;
                case "0":
                    backToMain = true;
                    break;
                default:
                    Console.WriteLine("Неверный выбор.");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nОшибка: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"   Детали: {ex.InnerException.Message}");
            }
        }

        if (!backToMain)
        {
            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }
    }
}

async Task ShowAllCarsAsync(ICarRepository repo)
{
    Console.WriteLine("Все автомобили\n");
    var cars = await repo.GetAllAsync();
    if (cars.Count == 0)
    {
        Console.WriteLine("В базе нет автомобилей.");
        return;
    }
    Console.WriteLine($"Найдено: {cars.Count}\n");
    foreach (var c in cars)
    {
        PrintCar(c);
    }
}
async Task AddNewCarAsync(ICarRepository repo, string connString)
{
    Console.WriteLine("Добавление нового автомобиля\n");

    Console.Write("VIN (17 символов): ");
    string vin = Console.ReadLine()?.Trim() ?? "";
    if (string.IsNullOrEmpty(vin))
    {
        Console.WriteLine("VIN не может быть пустым.");
        return;
    }
    if (vin.Length != 17)
    {
        Console.WriteLine($"VIN должен быть ровно 17 символов (сейчас {vin.Length}).");
        return;
    }
    Console.Write("Марка (например, Toyota): ");
    string brandName = Console.ReadLine()?.Trim() ?? "";
    if (string.IsNullOrEmpty(brandName))
    {
        Console.WriteLine("Марка не может быть пустой.");
        return;
    }
    Console.Write("Модель (например, Camry): ");
    string modelName = Console.ReadLine()?.Trim() ?? "";
    if (string.IsNullOrEmpty(modelName))
    {
        Console.WriteLine("Модель не может быть пустой.");
        return;
    }

    Console.WriteLine("\n Проверка марки...");
    Guid brandId = await repo.GetOrCreateBrandAsync(brandName);
    Console.WriteLine($"Марка '{brandName}' готова (ID: {brandId})");
    Console.WriteLine(" Проверка модели...");
    Guid modelId = await repo.GetOrCreateModelAsync(modelName, brandId);
    Console.WriteLine($" Модель '{modelName}' готова (ID: {modelId})");
    Console.Write("\nЦвет: ");
    string color = Console.ReadLine()?.Trim() ?? "Не указан";
    if (color.Length > 30)
    {
        Console.WriteLine(" Цвет обрезан до 30 символов.");
        color = color.Substring(0, 30);
    }
    Console.Write("Год выпуска: ");
    int year = ReadPositiveInt();
    if (year < 1900 || year > DateTime.Now.Year + 1)
    {
        Console.WriteLine($" Год должен быть от 1900 до {DateTime.Now.Year + 1}.");
        return;
    }

    Console.Write("Пробег (км): ");
    int mileage = ReadNonNegativeInt();

    Console.Write("Цена (руб., макс. 9 999 999 999): ");
    decimal price = ReadDecimal();
    if (price > 9999999999.99m)
    {
        Console.WriteLine(" Цена слишком большая! Максимум: 9 999 999 999.99 ₽");
        return;
    }

    Console.Write("Скидка (%) или Enter для пропуска: ");
    decimal? discount = null;
    string? discountInput = Console.ReadLine()?.Trim();
    if (!string.IsNullOrEmpty(discountInput))
    {
        if (decimal.TryParse(discountInput, out decimal d))
        {
            if (d < 0 || d > 100)
            {
                Console.WriteLine(" Скидка должна быть от 0 до 100%.");
                return;
            }
            discount = d;
        }
    }
    var newCar = new Car
    {
        VIN = vin,
        ModelId = modelId,
        Color = color,
        YearOfManufacture = year,
        Mileage = mileage,
        Price = price,
        DiscountPercent = discount
    };

    try
    {
        var created = await repo.CreateAsync(newCar);
        Console.WriteLine($"\n Автомобиль успешно добавлен!");
        Console.WriteLine($"   ID: {created.CarId}");
        Console.WriteLine($"   Марка/Модель: {brandName} {modelName}");
        Console.WriteLine($"   VIN: {created.VIN}");
        Console.WriteLine($"   Цвет: {created.Color}");
        Console.WriteLine($"   Год: {created.YearOfManufacture}");
        Console.WriteLine($"   Пробег: {created.Mileage} км");
        Console.WriteLine($"   Цена: {created.Price:N2} ₽");
        if (created.DiscountPercent.HasValue)
            Console.WriteLine($"   Скидка: {created.DiscountPercent}%");
        Console.WriteLine($"   Дата поступления: {created.ArrivalDate:dd.MM.yyyy HH:mm}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n Ошибка при добавлении: {ex.Message}");
        if (ex.InnerException != null)
        {
            Console.WriteLine($"   Детали: {ex.InnerException.Message}");
        }
    }
}

async Task UpdateCarAsync(ICarRepository repo)
{
    Console.Write("Введите ID автомобиля для изменения (GUID): ");
    Guid id = ReadGuid();

    var car = await repo.GetByIdAsync(id);
    if (car == null)
    {
        Console.WriteLine(" Автомобиль не найден.");
        return;
    }

    Console.WriteLine("\nТекущие данные:");
    PrintCar(car);

    Console.WriteLine("\nВведите новые значения (Enter — оставить без изменений):");

    Console.Write($"Цвет [{car.Color}]: ");
    string? colorInput = Console.ReadLine()?.Trim();
    if (!string.IsNullOrEmpty(colorInput)) car.Color = colorInput;

    Console.Write($"Год выпуска [{car.YearOfManufacture}]: ");
    string? yearInput = Console.ReadLine()?.Trim();
    if (!string.IsNullOrEmpty(yearInput) && int.TryParse(yearInput, out int y)) car.YearOfManufacture = y;

    Console.Write($"Пробег [{car.Mileage}]: ");
    string? mileageInput = Console.ReadLine()?.Trim();
    if (!string.IsNullOrEmpty(mileageInput) && int.TryParse(mileageInput, out int m)) car.Mileage = m;

    Console.Write($"Цена [{car.Price:N0}]: ");
    string? priceInput = Console.ReadLine()?.Trim();
    if (!string.IsNullOrEmpty(priceInput) && decimal.TryParse(priceInput, out decimal p)) car.Price = p;

    Console.Write($"Скидка [{car.DiscountPercent?.ToString() ?? "нет"}]: ");
    string? discountInput = Console.ReadLine()?.Trim();
    if (!string.IsNullOrEmpty(discountInput))
    {
        if (decimal.TryParse(discountInput, out decimal d)) car.DiscountPercent = d;
    }

    var updated = await repo.UpdateAsync(car);
    Console.WriteLine(updated ? "\n Автомобиль успешно обновлён!" : "\n Не удалось обновить.");
}

async Task DeleteCarAsync(ICarRepository repo)
{
    Console.Write("Введите ID автомобиля для удаления (GUID): ");
    Guid id = ReadGuid();

    // Проверяем наличие связанных записей
    bool hasSales = await repo.HasRelatedSalesAsync(id);

    if (hasSales)
    {
        Console.WriteLine("\n У этого автомобиля есть связанные продажи!");
        Console.WriteLine("   При удалении также будут удалены:");
        Console.WriteLine("    Все записи о продажах этого авто");
        Console.WriteLine("    Все связанные дополнительные услуги\n");
        Console.Write("Вы всё равно хотите удалить? (y/n): ");
    }
    else
    {
        Console.Write("Вы уверены? (y/n): ");
    }

    string? confirm = Console.ReadLine()?.Trim().ToLower();
    if (confirm != "y")
    {
        Console.WriteLine("Удаление отменено.");
        return;
    }

    try
    {
        var deleted = await repo.DeleteAsync(id);
        Console.WriteLine(deleted
            ? "\n Автомобиль и все связанные записи успешно удалены!"
            : "\n Автомобиль не найден.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n Ошибка при удалении: {ex.Message}");
        if (ex.InnerException != null)
        {
            Console.WriteLine($"   Детали: {ex.InnerException.Message}");
        }
    }
}


void PrintCar(Car c)
{
    string status = c.IsSold ? " Продан" : " В наличии";
    string discount = c.DiscountPercent.HasValue ? $"{c.DiscountPercent}%" : "нет";
    string finalPrice = c.FinalPrice.HasValue ? c.FinalPrice.Value.ToString("N0") : c.Price.ToString("N0");

    Console.WriteLine($"ID: {c.CarId}");
    Console.WriteLine($"Марка/Модель: {c.Model?.Brand?.BrandName ?? "?"} {c.Model?.ModelName ?? ""}");
    Console.WriteLine($"VIN: {c.VIN}");
    Console.WriteLine($"Цвет: {c.Color} | Год: {c.YearOfManufacture} | Пробег: {c.Mileage} км");
    Console.WriteLine($"Цена: {c.Price:N0} ₽ | Скидка: {discount} | Итого: {finalPrice} ₽");
    Console.WriteLine($"Поступил: {c.ArrivalDate:dd.MM.yyyy}");
    Console.WriteLine($"Статус: {status}\n");
}

Guid ReadGuid()
{
    while (true)
    {
        string? input = Console.ReadLine()?.Trim();
        if (Guid.TryParse(input, out Guid result))
        {
            return result;
        }
        Console.Write(" Некорректный GUID. Повторите ввод: ");
    }
}

int ReadPositiveInt()
{
    while (true)
    {
        string? input = Console.ReadLine()?.Trim();
        if (int.TryParse(input, out int result) && result > 0)
        {
            return result;
        }
        Console.Write(" Введите целое положительное число: ");
    }
}

int ReadNonNegativeInt()
{
    while (true)
    {
        string? input = Console.ReadLine()?.Trim();
        if (int.TryParse(input, out int result) && result >= 0)
        {
            return result;
        }
        Console.Write(" Введите целое неотрицательное число: ");
    }
}

decimal ReadDecimal()
{
    while (true)
    {
        string? input = Console.ReadLine()?.Trim();
        if (decimal.TryParse(input, out decimal result) && result >= 0)
        {
            return result;
        }
        Console.Write(" Введите число >= 0: ");
    }
}
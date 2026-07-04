using Microsoft.EntityFrameworkCore;
using AutoSalonCrud.Data;
using AutoSalonCrud.Entities;

namespace AutoSalonCrud.Repositories;

/// <summary>
/// Entity Framework
/// </summary>
public class EfCoreCarRepository : ICarRepository
{
    private readonly AutoSalonContext _context;

    public EfCoreCarRepository(AutoSalonContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<List<Car>> GetAllAsync()
    {
        return await _context.Cars
            .AsNoTracking()
            .Include(c => c.Model)
                .ThenInclude(m => m!.Brand)
            .OrderByDescending(c => c.ArrivalDate)
            .ToListAsync();
    }
    public async Task<Guid> GetOrCreateBrandAsync(string brandName)
    {
        var existing = await _context.Brands
            .FirstOrDefaultAsync(b => b.BrandName == brandName && b.IsActive);

        if (existing != null)
        {
            return existing.BrandId;
        }
        var newBrand = new Brand
        {
            BrandId = Guid.NewGuid(),
            BrandName = brandName,
            Country = "Не указана",
            IsActive = true,
            CreatedAt = DateTime.Now
        };

        _context.Brands.Add(newBrand);
        await _context.SaveChangesAsync();

        return newBrand.BrandId;
    }

    public async Task<Guid> GetOrCreateModelAsync(string modelName, Guid brandId)
    {
        var existing = await _context.Models
            .FirstOrDefaultAsync(m => m.ModelName == modelName && m.BrandId == brandId && m.IsActive);

        if (existing != null)
        {
            return existing.ModelId;
        }

        var newModel = new Model
        {
            ModelId = Guid.NewGuid(),
            ModelName = modelName,
            BrandId = brandId,
            BodyType = "Не указан",
            BasePrice = 0,
            IsActive = true
        };

        _context.Models.Add(newModel);
        await _context.SaveChangesAsync();

        return newModel.ModelId;
    }
    public async Task<Car?> GetByIdAsync(Guid id)
    {
        return await _context.Cars
            .AsNoTracking()
            .Include(c => c.Model)
                .ThenInclude(m => m!.Brand)
            .FirstOrDefaultAsync(c => c.CarId == id);
    }

    public async Task<Car> CreateAsync(Car car)
    {
        try
        {
            if (car.CarId == Guid.Empty)
            {
                car.CarId = Guid.NewGuid();
            }
            if (car.ArrivalDate == default)
            {
                car.ArrivalDate = DateTime.Now;
            }
            var modelExists = await _context.Models.AnyAsync(m => m.ModelId == car.ModelId);
            if (!modelExists)
            {
                throw new InvalidOperationException($"Модель с ID {car.ModelId} не найдена.");
            }
            var vinExists = await _context.Cars.AnyAsync(c => c.VIN == car.VIN);
            if (vinExists)
            {
                throw new InvalidOperationException($"Автомобиль с VIN {car.VIN} уже существует.");
            }

            _context.Cars.Add(car);
            var affected = await _context.SaveChangesAsync();

            if (affected == 0)
            {
                throw new InvalidOperationException("Не удалось сохранить автомобиль в БД.");
            }

            return car;
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException(
                $"Ошибка БД: {ex.InnerException?.Message ?? ex.Message}", ex);
        }
    }
    public async Task<bool> UpdateAsync(Car car)
    {
        var existing = await _context.Cars.FindAsync(car.CarId);
        if (existing == null) return false;

        existing.Color = car.Color;
        existing.YearOfManufacture = car.YearOfManufacture;
        existing.Mileage = car.Mileage;
        existing.Price = car.Price;
        existing.DiscountPercent = car.DiscountPercent;

        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> HasRelatedSalesAsync(Guid carId)
    {
        return await _context.Sales.AnyAsync(s => s.CarId == carId);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return false;
            }

            var salesIds = await _context.Sales
                .Where(s => s.CarId == id)
                .Select(s => s.SaleId)
                .ToListAsync();

            if (salesIds.Any())
            {
                var salesServices = await _context.SalesServices
                    .Where(ss => salesIds.Contains(ss.SaleId))
                    .ToListAsync();
                _context.SalesServices.RemoveRange(salesServices);
            }
            var sales = await _context.Sales
                .Where(s => s.CarId == id)
                .ToListAsync();
            _context.Sales.RemoveRange(sales);
            _context.Cars.Remove(car);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
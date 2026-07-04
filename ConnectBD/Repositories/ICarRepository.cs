using AutoSalonCrud.Entities;

namespace AutoSalonCrud.Repositories;

public interface ICarRepository
{
    Task<List<Car>> GetAllAsync();
    Task<Car?> GetByIdAsync(Guid id);
    Task<Car> CreateAsync(Car car);
    Task<bool> UpdateAsync(Car car);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> HasRelatedSalesAsync(Guid carId);
    Task<Guid> GetOrCreateBrandAsync(string brandName);
    Task<Guid> GetOrCreateModelAsync(string modelName, Guid brandId);
}
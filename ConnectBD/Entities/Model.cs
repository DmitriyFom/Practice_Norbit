namespace AutoSalonCrud.Entities;

/// <summary>
/// Модель авто.
/// </summary>
public class Model
{
    public Guid ModelId { get; set; }
    public string ModelName { get; set; } = string.Empty;
    public Guid BrandId { get; set; }
    public string BodyType { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public bool IsActive { get; set; }

    public Brand? Brand { get; set; }
    public ICollection<Car> Cars { get; set; } = new List<Car>();
}
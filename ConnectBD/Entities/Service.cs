namespace AutoSalonCrud.Entities;

/// <summary>
/// Дополнительная услуга.
/// </summary>
public class Service
{
    public Guid ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
    public string? Description { get; set; }

    public ICollection<SalesService> SalesServices { get; set; } = new List<SalesService>();
}
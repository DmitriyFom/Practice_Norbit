namespace AutoSalonCrud.Entities;

/// <summary>
/// Салон авто.
/// </summary>
public class Car
{
    public Guid CarId { get; set; }
    public string VIN { get; set; } = string.Empty;
    public Guid ModelId { get; set; }
    public string Color { get; set; } = string.Empty;
    public int YearOfManufacture { get; set; }
    public int Mileage { get; set; }
    public decimal Price { get; set; }
    public bool IsSold { get; set; }
    public DateTime ArrivalDate { get; set; }
    public decimal? DiscountPercent { get; set; }

    public Model? Model { get; set; }

    public decimal? FinalPrice => DiscountPercent.HasValue
        ? Price * (1 - DiscountPercent.Value / 100)
        : Price;
}
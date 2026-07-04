namespace AutoSalonCrud.Entities;

/// <summary>
/// Дополнительная услуга к продаже.
/// </summary>
public class SalesService
{
    public Guid SaleId { get; set; }
    public Guid ServiceId { get; set; }
    public decimal ServicePrice { get; set; }
    public Sale? Sale { get; set; }
    public Service? Service { get; set; }
}
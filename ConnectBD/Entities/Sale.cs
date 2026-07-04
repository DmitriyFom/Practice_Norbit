namespace AutoSalonCrud.Entities;

/// <summary>
/// Продажа автомобиля.
/// </summary>
public class Sale
{
    public Guid SaleId { get; set; }
    public Guid CarId { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime SaleDate { get; set; }
    public decimal FinalPrice { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public Car? Car { get; set; }
    public Customer? Customer { get; set; }
    public ICollection<SalesService> SalesServices { get; set; } = new List<SalesService>();
}
namespace AutoSalonCrud.Entities;

/// <summary>
/// Клиент.
/// </summary>
public class Customer
{
    public Guid CustomerId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public bool IsVIP { get; set; }
    public DateTime RegistrationDate { get; set; }
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
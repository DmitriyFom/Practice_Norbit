namespace AutoSalonCrud.Entities;

/// <summary>
/// Марка авто.
/// </summary>
public class Brand
{
    public Guid BrandId { get; set; }
    public string BrandName { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int? FoundedYear { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<Model> Models { get; set; } = new List<Model>();
}
namespace TGProV4.Domain.Entities;

public class Product : AuditableEntity<int>
{
    public string? Name { get; set; } = default;
    public string? Barcode { get; set; } = default;
    public string? Description { get; set; } = default;
}
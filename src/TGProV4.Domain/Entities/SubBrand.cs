namespace TGProV4.Domain.Entities;

public class SubBrand : AuditableEntity<int>
{
    public string? Name { get; set; }
    public string? Description { get; set; }

    public int? BrandId { get; set; }
    public virtual Brand? Brand { get; set; }

    public int? CategoryId { get; set; }
    public virtual Category? Category { get; set; }

    public virtual ICollection<Product>? Products { get; set; }
}

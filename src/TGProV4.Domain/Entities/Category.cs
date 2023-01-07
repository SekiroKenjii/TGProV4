namespace TGProV4.Domain.Entities;

public class Category : AuditableEntity<int>
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    
    public virtual ICollection<SubBrand>? SubBrands { get; set; }
    public virtual ICollection<Product>? Products { get; set; }
}

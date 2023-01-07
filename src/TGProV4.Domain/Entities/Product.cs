namespace TGProV4.Domain.Entities;

public class Product : AuditableEntity<int>
{
    public string? Name { get; set; }
    
    public int? SubBrandId { get; set; }
    public virtual SubBrand? SubBrand { get; set; }
    
    public int? CategoryId { get; set; }
    public virtual Category? Category { get; set; }

    public virtual ICollection<ProductDetail>? ProductDetails { get; set; }
}

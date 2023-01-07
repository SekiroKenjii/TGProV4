namespace TGProV4.Domain.Entities;

public class ProductDetail : AuditableEntity<int>
{
    public string? Sku { get; set; }
    public string? Specification { get; set; }
    public string? Description { get; set; }
    public string? Warranty { get; set; }
    public decimal Price { get; set; }
    public decimal OriginalPrice { get; set; }
    public int UnitsInStock { get; set; }
    public int UnitsOnOrder { get; set; }
    public bool Discontinued { get; set; }

    public int? ProductId { get; set; }
    public virtual Product? Product { get; set; }

    public int? ConditionId { get; set; }
    public virtual ProductCondition? Condition { get; set; }
    
    public int? TypeId { get; set; }
    public virtual ProductType? Type { get; set; }
    
    public virtual ICollection<ProductImage>? Images { get; set; }
    public virtual ICollection<ProductColor>? Colors { get; set; }
}

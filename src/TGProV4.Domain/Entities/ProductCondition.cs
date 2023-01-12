namespace TGProV4.Domain.Entities;

public class ProductCondition : AuditableEntity<int>
{
    public string? Name { get; set; }
    public string? Description { get; set; }

    public virtual ICollection<ProductDetail>? Products { get; set; }
}

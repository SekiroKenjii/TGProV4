using System.Collections;

namespace TGProV4.Domain.Entities;

public class ProductType : AuditableEntity<int>
{
    public string? Name { get; set; }
    public string? Description { get; set; }

    public virtual ICollection<ProductDetail>? Products { get; set; }
}

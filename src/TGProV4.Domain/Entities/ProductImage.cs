namespace TGProV4.Domain.Entities;

public class ProductImage : AuditableEntity<int>
{
    public string? ImageUrl { get; set; }
    public string? ImageId { get; set; }
    public string? Caption { get; set; }
    public int SortOrder { get; set; }

    public int? ProductId { get; set; }
    public virtual ProductDetail? Product { get; set; }
}

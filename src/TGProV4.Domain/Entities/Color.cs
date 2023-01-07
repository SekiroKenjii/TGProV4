namespace TGProV4.Domain.Entities;

public class Color : AuditableEntity<int>
{
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Type { get; set; }

    public virtual ICollection<ProductColor>? ProductColors { get; set; }
}

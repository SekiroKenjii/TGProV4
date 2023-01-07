namespace TGProV4.Domain.Entities;

public class ProductColor : IEntity<int>
{
    public int Id { get; set; }

    public int? ProductId { get; set; }
    public ProductDetail? Product { get; set; }

    public int? ColorId { get; set; }
    public Color? Color { get; set; }
}

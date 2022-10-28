namespace TGProV4.Domain.Contracts;

public interface IEntity
{
}

public interface IEntity<TId> : IEntity
{
    public TId Id { get; set; }
}
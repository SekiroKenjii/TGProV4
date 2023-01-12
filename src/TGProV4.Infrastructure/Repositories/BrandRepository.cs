namespace TGProV4.Infrastructure.Repositories;

public class BrandRepository : IBrandRepository
{
    private readonly IRepositoryBase<Brand, int> _repository;

    public BrandRepository(IRepositoryBase<Brand, int> repository) => _repository = repository;
}

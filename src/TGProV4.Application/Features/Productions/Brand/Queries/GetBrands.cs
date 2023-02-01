namespace TGProV4.Application.Features.Productions.Brand.Queries;

public class GetBrands
{
    public class Query : IRequest<IEnumerable<BrandResponse>> {}

    // ReSharper disable once UnusedType.Global
    public class Handler : IRequestHandler<Query, IEnumerable<BrandResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IAppCache _cache;

        public Handler(IUnitOfWork<int> unitOfWork, IAppCache cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<IEnumerable<BrandResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            async Task<IEnumerable<BrandResponse>> Brands()
            {
                return await _unitOfWork.Repository<Domain.Entities.Brand>()
                                        .GetEntities<BrandResponse>(s => new BrandResponse {
                                             Id = s.Id,
                                             Name = s.Name,
                                             Description = s.Description,
                                             LogoId = s.LogoId,
                                             LogoUrl = s.LogoUrl
                                         });
            }

            return await _cache.GetOrAddAsync(ApplicationConstants.Cache.AllBrandsCacheKey, Brands);
        }
    }
}

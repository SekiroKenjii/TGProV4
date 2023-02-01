namespace TGProV4.Application.Features.Productions.Brand.Queries;

public class GetBrands
{
    public class Query : IRequest<List<BrandResponse>> {}

    // ReSharper disable once UnusedType.Global
    public class Handler : IRequestHandler<Query, List<BrandResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;

        public Handler(IUnitOfWork<int> unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<List<BrandResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Repository<Domain.Entities.Brand>()
                                    .GetEntities<BrandResponse>(s => new BrandResponse {
                                         Id = s.Id,
                                         Name = s.Name,
                                         Description = s.Description,
                                         LogoId = s.LogoId,
                                         LogoUrl = s.LogoUrl
                                     })
                                    .ToListAsync(cancellationToken);
        }
    }
}

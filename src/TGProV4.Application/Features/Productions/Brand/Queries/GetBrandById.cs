namespace TGProV4.Application.Features.Productions.Brand.Queries;

public class GetBrandById
{
    public class Query : IRequest<BrandResponse>
    {
        public int Id { get; init; }
    }

    // ReSharper disable once UnusedType.Global
    public class Handler : IRequestHandler<Query, BrandResponse?>
    {
        private readonly IUnitOfWork<int> _unitOfWork;

        public Handler(IUnitOfWork<int> unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<BrandResponse?> Handle(Query request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Repository<Domain.Entities.Brand>()
                                    .GetEntity<BrandResponse>(
                                         p => p.Id == request.Id,
                                         s => new BrandResponse {
                                             Id = s.Id,
                                             Name = s.Name,
                                             Description = s.Description,
                                             LogoId = s.LogoId,
                                             LogoUrl = s.LogoUrl
                                         },
                                         cancellationToken);
        }
    }
}

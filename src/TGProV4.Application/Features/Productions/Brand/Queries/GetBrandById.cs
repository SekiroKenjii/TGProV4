namespace TGProV4.Application.Features.Productions.Brand.Queries;

public class GetBrandById
{
    public class Query : IRequest<BrandResponse>
    {
        public int Id { get; init; }
    }

    internal class Handler : IRequestHandler<Query, BrandResponse?>
    {
        private readonly IUnitOfWork<int> _unitOfWork;

        public Handler(IUnitOfWork<int> unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<BrandResponse?> Handle(Query request, CancellationToken cancellationToken)
        {
            return await _unitOfWork
                        .Repository<Domain.Entities.Brand>()
                        .GetEntities(x => x.Id == request.Id)
                        .Select(x => new BrandResponse {
                             Id = x.Id,
                             Name = x.Name,
                             Description = x.Description,
                             LogoId = x.LogoId,
                             LogoUrl = x.LogoUrl
                         })
                        .FirstOrDefaultAsync(cancellationToken);
        }
    }
}

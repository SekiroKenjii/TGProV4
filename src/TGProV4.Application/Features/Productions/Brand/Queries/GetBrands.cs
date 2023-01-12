namespace TGProV4.Application.Features.Productions.Brand.Queries;

public class GetBrands
{
    public class Query : IRequest<List<QueryBrandResponse>> {}

    internal class Handler : IRequestHandler<Query, List<QueryBrandResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;

        public Handler(IUnitOfWork<int> unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<List<QueryBrandResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            return await _unitOfWork
                        .Repository<Domain.Entities.Brand>()
                        .GetEntities()
                        .Select(x => new QueryBrandResponse {
                             Id = x.Id,
                             Name = x.Name,
                             Description = x.Description,
                             LogoId = x.LogoId,
                             LogoUrl = x.LogoUrl
                         })
                        .ToListAsync(cancellationToken);
        }
    }
}

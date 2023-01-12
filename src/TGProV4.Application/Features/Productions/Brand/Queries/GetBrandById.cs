namespace TGProV4.Application.Features.Productions.Brand.Queries;

public class GetBrandById
{
    public class Query : IRequest<QueryBrandResponse>
    {
        public int Id { get; init; }
    }

    internal class Handler : IRequestHandler<Query, QueryBrandResponse?>
    {
        private readonly IUnitOfWork<int> _unitOfWork;

        public Handler(IUnitOfWork<int> unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<QueryBrandResponse?> Handle(Query request, CancellationToken cancellationToken)
        {
            return await _unitOfWork
                        .Repository<Domain.Entities.Brand>()
                        .GetEntities(x => x.Id == request.Id)
                        .Select(x => new QueryBrandResponse {
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

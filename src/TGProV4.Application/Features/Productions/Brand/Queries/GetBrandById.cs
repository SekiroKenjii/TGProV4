using TGProV4.Shared.Helpers;

namespace TGProV4.Application.Features.Productions.Brand.Queries;

public class GetBrandById : IRequest<QueryBrandResponse>
{
    public int Id { get; init; }
}

internal class GetBrandByIdHandler : IRequestHandler<GetBrandById, QueryBrandResponse>
{
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IMapper _mapper;

    public GetBrandByIdHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<QueryBrandResponse> Handle(GetBrandById request, CancellationToken cancellationToken)
    {
        var brand = await _unitOfWork.Repository<Domain.Entities.Brand>()
            .GetEntities(x => x.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (brand == null) throw new NotFoundException(StringHelpers.Message.NotFound("Brand"));

        return _mapper.Map<QueryBrandResponse>(brand);
    }
}

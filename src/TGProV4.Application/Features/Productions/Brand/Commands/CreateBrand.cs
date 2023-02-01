namespace TGProV4.Application.Features.Productions.Brand.Commands;

public class CreateBrand
{
    public class Command : IRequest<BrandResponse>
    {
        public UpsertBrandRequest? Brand { get; init; }
    }

    // ReSharper disable once UnusedType.Global
    public class Handler : IRequestHandler<Command, BrandResponse?>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IImageService<UpsertBrandRequest> _imageService;
        private readonly ICurrentUserService _currentUserService;

        public Handler(IUnitOfWork<int> unitOfWork,
                       IMapper mapper,
                       IImageService<UpsertBrandRequest> imageService,
                       ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _imageService = imageService;
            _currentUserService = currentUserService;
        }

        public async Task<BrandResponse?> Handle(Command request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_currentUserService.UserId))
            {
                throw new UnauthorizedException(ApplicationConstants.Messages.Unauthorized);
            }

            if (request.Brand is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.Brand.Entity = ApplicationConstants.Entities.Brand;

            var imageUpload = await _imageService.Upload(request.Brand);

            var brand = _unitOfWork.Repository<Domain.Entities.Brand>()
                                   .AddAsync(new Domain.Entities.Brand {
                                        Name = request.Brand.Name,
                                        Description = request.Brand.Description,
                                        LogoUrl = imageUpload.ImageUrl,
                                        LogoId = imageUpload.PublicId,
                                        CreatedBy = _currentUserService.UserId
                                    });

            var save = await _unitOfWork.CommitAndRemoveCache(cancellationToken,
                           ApplicationConstants.Cache.AllBrandsCacheKey) > 0;

            return !save ? default : _mapper.Map<BrandResponse>(brand);
        }
    }
}

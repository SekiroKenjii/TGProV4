using TGProV4.Shared.Helpers;

namespace TGProV4.Application.Features.Productions.Brand.Commands;

public class UpdateBrand
{
    public class Command : IRequest<bool>
    {
        public int Id { get; init; }
        public UpsertBrandRequest? Brand { get; init; }
    }

    // ReSharper disable once UnusedType.Global
    public class Handler : IRequestHandler<Command, bool>
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

        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_currentUserService.UserId))
            {
                throw new UnauthorizedException(ApplicationConstants.Messages.Unauthorized);
            }

            if (request.Brand is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var brand = await _unitOfWork.Repository<Domain.Entities.Brand>()
                                         .GetEntity(x => x.Id == request.Id);

            if (brand is null)
            {
                throw new NotFoundException(StringHelpers.Message.NotFound("Brand"));
            }

            brand = _mapper.Map<Domain.Entities.Brand>(request.Brand);

            if (request.Brand.ImageFile is not null)
            {
                request.Brand.Entity = ApplicationConstants.Entities.Brand;

                if (!string.IsNullOrEmpty(brand.LogoId) &&
                    brand.LogoId is not ApplicationConstants.DefaultImages.BrandImageId)
                {
                    await _imageService.Remove(brand.LogoId);
                }

                var imageUpload = await _imageService.Upload(request.Brand);

                brand.LogoId = imageUpload.PublicId;
                brand.LogoUrl = imageUpload.ImageUrl;
            }

            await _unitOfWork.Repository<Domain.Entities.Brand>().UpdateAsync(brand);

            return await _unitOfWork.Commit(cancellationToken) > 0;
        }
    }
}

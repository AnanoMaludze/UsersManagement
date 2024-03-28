using AutoMapper;
using ErrorOr;
using MediatR;
using UsersManagement.Entities;
using UsersManagement.Repositories;
using UsersManagement.Services;

namespace UsersManagement.Handlers.Commands
{
    public class UploadOrChangeImageCommand : IRequest<ErrorOr<string>>
    {
        public int PersonId { get; }
        public IFormFile ImageFile { get; }

        public UploadOrChangeImageCommand(int personId, IFormFile imageFile)
        {
            PersonId = personId;
            ImageFile = imageFile;
        }

        public class UploadImageCommandHandler : IRequestHandler<UploadOrChangeImageCommand, ErrorOr<string>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;
            private readonly IFileStorageService _fileStorageService;

            public UploadImageCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IFileStorageService fileStorageService)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _fileStorageService = fileStorageService;
            }

            public async Task<ErrorOr<string>> Handle(UploadOrChangeImageCommand request, CancellationToken cancellationToken)
            {
                var person = await _unitOfWork.Repository().GetByIdWithIncludes<Person>(request.PersonId);
                if (person == null)
                {
                    return Error.NotFound("PersonNotFound", "The person could not be found.");
                }
                if (person.Image != null)
                {
                    await _fileStorageService.DeleteImageAsync(person.Image);
                }
                try
                {
                    var imagePath = await _fileStorageService.SaveImageAsync(request.ImageFile);

                    person.Image = imagePath;

                    _unitOfWork.Repository().Update(person);
                    await _unitOfWork.CommitAsync(cancellationToken);

                    return imagePath;
                }
                catch (Exception ex)
                {
                    return Error.Failure("ImageUploadFailed", $"Failed to upload image. Error: {ex.Message}");
                }
            }
        }

    }


}

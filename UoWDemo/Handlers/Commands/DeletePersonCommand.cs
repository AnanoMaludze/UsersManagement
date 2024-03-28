using ErrorOr;
using MediatR;
using UsersManagement.Entities;
using UsersManagement.Repositories;

namespace UsersManagement.Handlers.Commands
{
    public class DeletePersonCommand : IRequest<ErrorOr<bool>>
    {
        public int PersonId { get; }

        public DeletePersonCommand(int personId)
        {
            PersonId = personId;
        }
    }

    public class DeletePersonCommandHandler : IRequestHandler<DeletePersonCommand, ErrorOr<bool>>
    {
        private readonly IUnitOfWork _uow;

        public DeletePersonCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ErrorOr<bool>> Handle(DeletePersonCommand request, CancellationToken cancellationToken)
        {
            var person = await _uow.Repository().GetById<Person>(request.PersonId);
            if (person == null)
            {
                return false;
            }

            _uow.Repository().Delete(person);
            await _uow.CommitAsync(cancellationToken);

            return true;
        }
    }

}

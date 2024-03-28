using ErrorOr;
using MediatR;
using UsersManagement.Entities;
using UsersManagement.Enums;
using UsersManagement.Repositories;

namespace UsersManagement.Handlers.Commands
{
    public class AddRelationshipCommand : IRequest<ErrorOr<bool>>
    {
        public int PersonId { get; }
        public int RelatedPersonId { get; }
        public RelationshipType RelationshipType { get; }

        public AddRelationshipCommand(int personId, int relatedPersonId, RelationshipType relationshipType)
        {
            PersonId = personId;
            RelatedPersonId = relatedPersonId;
            RelationshipType = relationshipType;
        }
    }

    public class AddRelationshipCommandHandler : IRequestHandler<AddRelationshipCommand, ErrorOr<bool>>
    {
        private readonly IUnitOfWork _uow;

        public AddRelationshipCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ErrorOr<bool>> Handle(AddRelationshipCommand request, CancellationToken cancellationToken)
        {
            var relationship = new Relationship
            {
                PersonId = request.PersonId,
                RelatedPersonId = request.RelatedPersonId,
                RelationshipType = request.RelationshipType
            };

            _uow.Repository().Add(relationship);
            await _uow.CommitAsync(cancellationToken);

            return true;
        }
    }

}

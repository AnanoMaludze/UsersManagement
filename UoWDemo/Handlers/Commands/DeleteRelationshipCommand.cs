using ErrorOr;
using MediatR;
using System;
using UsersManagement.Entities;
using UsersManagement.Repositories;

namespace UsersManagement.Handlers.Commands
{
    public class DeleteRelationshipCommand : IRequest<ErrorOr<bool>>
    {
        public int RelationshipId { get; }

        public DeleteRelationshipCommand(int relationshipId)
        {
            RelationshipId = relationshipId;
        }
    }

    public class DeleteRelationshipCommandHandler : IRequestHandler<DeleteRelationshipCommand, ErrorOr<bool>>
    {
        private readonly IUnitOfWork _uow;

        public DeleteRelationshipCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ErrorOr<bool>> Handle(DeleteRelationshipCommand request, CancellationToken cancellationToken)
        {
            var relationship = await _uow.Repository().GetById<Relationship>(request.RelationshipId);

            if (relationship == null)
            {
                return false;
            }

            _uow.Repository().Delete(relationship);
            await _uow.CommitAsync(cancellationToken);

            return true;
        }
    }

}

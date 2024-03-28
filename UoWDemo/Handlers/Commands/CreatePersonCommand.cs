using AutoMapper;
using ErrorOr;
using MediatR;
using UsersManagement.Entities;
using UsersManagement.Enums;
using UsersManagement.Repositories;
using UsersManagement.Resources;

namespace UsersManagement.Handlers.Commands
{
    public class CreatePersonCommand : IRequest<ErrorOr<PersonResource>>
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public GenderType Gender { get; set; }
        public string PersonalNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int CityId { get; set; }
        public List<PhoneNumberDto> PhoneNumbers { get; set; } = new List<PhoneNumberDto>();
        public List<RelatedIndividualDto> RelatedIndividuals { get; set; } = new List<RelatedIndividualDto>();

        public class PhoneNumberDto
        {
            public PhoneNumberType NumberType { get; set; }
            public string Number { get; set; }
        }

        public class RelatedIndividualDto
        {
            public RelationshipType TypeOfConnection { get; set; }
            public int RelatedPersonId { get; set; }
        }

        public class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand, ErrorOr<PersonResource>>
        {
            private readonly IUnitOfWork _uow;
            private readonly IMapper _mapper;

            public CreatePersonCommandHandler(IUnitOfWork uow, IMapper mapper)
            {
                _uow = uow;
                _mapper = mapper;
            }

            public async Task<ErrorOr<PersonResource>> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
            {
                await _uow.BeginTransactionAsync(cancellationToken);

                try
                {
                    var person = _mapper.Map<Person>(request);
                    _uow.Repository().Add(person);
                    await _uow.CommitAsync(cancellationToken);


                    foreach (var phoneNumberDto in request.PhoneNumbers)
                    {
                        var phoneNumber = _mapper.Map<PhoneNumber>(phoneNumberDto);
                        phoneNumber.PersonId = person.Id;
                        _uow.Repository().Add(phoneNumber);
                    }


                    foreach (var relatedIndividualDto in request.RelatedIndividuals)
                    {
                        var relationship = _mapper.Map<Relationship>(relatedIndividualDto);
                        relationship.PersonId = person.Id;
                        _uow.Repository().Add(relationship);
                    }

                    await _uow.CommitAsync(cancellationToken);


                    await _uow.CommitTransactionAsync(cancellationToken);

                    var personResource = _mapper.Map<PersonResource>(person);
                    return personResource;
                }
                catch (Exception ex)
                {
                    await _uow.RollbackTransactionAsync(cancellationToken);
                    return Error.Failure(code: "CreatePersonFailed", description: $"Failed to create a new person. Error: {ex.Message}");
                }
            }
        }
    }
}

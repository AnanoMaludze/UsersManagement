using AutoMapper;
using ErrorOr;
using MediatR;
using System;
using UsersManagement.Entities;
using UsersManagement.Enums;
using UsersManagement.Repositories;
using UsersManagement.Resources;

namespace UsersManagement.Handlers.Commands
{
    public class UpdatePersonCommand : IRequest<ErrorOr<PersonResource>>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public GenderType Gender { get; set; }
        public string PersonalNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int CityId { get; set; }
        public List<UpPhoneNumberDto> PhoneNumbers { get; set; } = new List<UpPhoneNumberDto>();
    
        public class UpPhoneNumberDto
        {
            public int? Id { get; set; }
            public PhoneNumberType NumberType { get; set; }
            public string Number { get; set; }
        }

        public class UpRelatedIndividualDto
        {
            public RelationshipType TypeOfConnection { get; set; }
            public int RelatedPersonId { get; set; }
        }

        public class UpdatePersonCommandHandler : IRequestHandler<UpdatePersonCommand, ErrorOr<PersonResource>>
        {
            private readonly IUnitOfWork _uow;
            private readonly IMapper _mapper;

            public UpdatePersonCommandHandler(IUnitOfWork uow, IMapper mapper)
            {
                _uow = uow;
                _mapper = mapper;
            }

            public async Task<ErrorOr<PersonResource>> Handle(UpdatePersonCommand request, CancellationToken cancellationToken)
            {
                await _uow.BeginTransactionAsync(cancellationToken);

                try
                {
                    var person = await _uow.Repository().GetByIdWithIncludes<Person>(request.Id);

                    if (person == null)
                    {
                        await _uow.RollbackTransactionAsync(cancellationToken);
                        return Error.NotFound("Person not found");
                    }

                    _mapper.Map(request, person); 
                    UpdatePhoneNumbers(person.PhoneNumbers, request.PhoneNumbers);
                    //UpdateRelatedIndividuals(person.Relationships, request.RelatedIndividuals);

                    _uow.Repository().Update(person);
                    
                    await _uow.CommitAsync(cancellationToken);
                    await _uow.CommitTransactionAsync(cancellationToken);

                    var personResource = _mapper.Map<PersonResource>(person);
                    return personResource;
                }
                catch (Exception ex)
                {
                    await _uow.RollbackTransactionAsync(cancellationToken);
                    return Error.Failure(code: "UpdateFailed", description: $"Failed to update person. Error: {ex.Message}");
                }
            }
             private void UpdatePhoneNumbers(ICollection<PhoneNumber> existingPhoneNumbers, List<UpdatePersonCommand.UpPhoneNumberDto> updatedPhoneNumbers)
            {
                var updatedPhoneNumberIds = updatedPhoneNumbers.Where(upn => upn.Id.HasValue).Select(upn => upn.Id.Value).ToList();

                var phoneNumbersToRemove = existingPhoneNumbers
                    .Where(epn => !updatedPhoneNumberIds.Contains(epn.Id))
                    .ToList();

                foreach (var phoneNumber in phoneNumbersToRemove)
                {
                    existingPhoneNumbers.Remove(phoneNumber);
                    _uow.Repository().Delete(phoneNumber);
                }

                foreach (var updatedPhoneNumber in updatedPhoneNumbers)
                {
                    if (updatedPhoneNumber.Id.HasValue)
                    {
                        var existingPhoneNumber = existingPhoneNumbers.FirstOrDefault(epn => epn.Id == updatedPhoneNumber.Id.Value);
                        if (existingPhoneNumber != null)
                        {
                            _mapper.Map(updatedPhoneNumber, existingPhoneNumber);
                            _uow.Repository().UpdateRange(existingPhoneNumbers);
                        }
                        }
                    else
                    {
                        var newPhoneNumber = _mapper.Map<PhoneNumber>(updatedPhoneNumber);
                        existingPhoneNumbers.Add(newPhoneNumber);
                        _uow.Repository().Add(newPhoneNumber);
                    }
                }
          

            }


        }
    }



}

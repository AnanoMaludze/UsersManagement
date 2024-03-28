using AutoMapper;
using UsersManagement.Entities;
using UsersManagement.Handlers.Commands;
using UsersManagement.Resources;

namespace UsersManagement.Mapper
{
    public class PersonProfile : Profile
    {
        public PersonProfile()
        {
            CreateMap<PhoneNumber, PhoneNumberResource>()
            .ForMember(dest => dest.NumberType, opt => opt.MapFrom(src => src.NumberType.ToString()))
            .ReverseMap();

            CreateMap<City, CityResource>()
                .ReverseMap();

            CreateMap<Relationship, RelationshipResource>()
                .ForMember(dest => dest.RelationshipType, opt => opt.MapFrom(src => src.RelationshipType.ToString()))
                .ReverseMap();

            CreateMap<Person, PersonResource>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Surname))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender.ToString()))
                .ForMember(dest => dest.PersonalNumber, opt => opt.MapFrom(src => src.PersonalNumber))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
                .ForMember(dest => dest.CityId, opt => opt.MapFrom(src => src.CityId))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.PhoneNumbers, opt => opt.MapFrom(src => src.PhoneNumbers))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src.Relationships))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ReverseMap();

            CreateMap<CreatePersonCommand, Person>()
           .ForMember(dest => dest.PhoneNumbers, opt => opt.MapFrom(src => src.PhoneNumbers))
           .ForMember(dest => dest.Relationships, opt => opt.Ignore());


            CreateMap<CreatePersonCommand.PhoneNumberDto, PhoneNumber>();


            CreateMap<CreatePersonCommand.RelatedIndividualDto, Relationship>()
                .ForMember(dest => dest.RelationshipType, opt => opt.MapFrom(src => src.TypeOfConnection))
                .ForMember(dest => dest.RelatedPersonId, opt => opt.MapFrom(src => src.RelatedPersonId))
                .ForMember(dest => dest.PersonId, opt => opt.Ignore());


            CreateMap<UpdatePersonCommand, Person>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PhoneNumbers, opt => opt.MapFrom(src => src.PhoneNumbers))
                .ForMember(dest => dest.Relationships, opt => opt.Ignore());


            CreateMap<UpdatePersonCommand.UpPhoneNumberDto, PhoneNumber>();


            CreateMap<UpdatePersonCommand.UpRelatedIndividualDto, Relationship>()
                .ForMember(dest => dest.RelationshipType, opt => opt.MapFrom(src => src.TypeOfConnection))
                .ForMember(dest => dest.RelatedPersonId, opt => opt.MapFrom(src => src.RelatedPersonId))
                .ForMember(dest => dest.PersonId, opt => opt.Ignore());

        }
    }
}

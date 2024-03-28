using UsersManagement.Entities;

namespace UsersManagement.Resources
{
    public class PersonResource
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string PersonalNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int CityId { get; set; }
        public CityResource City { get; set; }
        public IEnumerable<PhoneNumberResource> PhoneNumbers { get; set; }
        public string Image { get; set; }
        public IEnumerable<RelationshipResource> Relationships { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }


    public class PhoneNumberResource
    {
        public int Id { get; set; }
        public string NumberType { get; set; }
        public string Number { get; set; }
    }

    public class CityResource
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class RelationshipResource
    {
        public int Id { get; set; }
        public string RelationshipType { get; set; } 
        public int PersonId { get; set; }
        public int RelatedPersonId { get; set; }
        public PersonResource RelatedPerson { get; set; } 
    }


}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UsersManagement.Enums;

namespace UsersManagement.Entities
{
  
	
public record Person : IEntity
        {

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Surname { get; set; }

        [Required]
        public GenderType Gender { get; set; }

        [Required]
        [StringLength(11, MinimumLength = 11)]
        public string PersonalNumber { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [ForeignKey("City")]
        public int CityId { get; set; }
        public virtual City City { get; set; }

        public virtual ICollection<PhoneNumber> PhoneNumbers { get; set; }

        public string? Image { get; set; }

        public virtual ICollection<Relationship> Relationships { get; set; }
    }

    public record PhoneNumber : IEntity
    {
       
        public PhoneNumberType NumberType { get; set; } 

        [StringLength(50, MinimumLength = 4)]
        public string Number { get; set; }

        [ForeignKey("Person")]
        public int PersonId { get; set; }
        public Person Person { get; set; }
    }

    public record Relationship : IEntity
    {
       
        public RelationshipType RelationshipType { get; set; }
        public int PersonId { get; set; }
        public int RelatedPersonId { get; set; }

        [ForeignKey("PersonId")]
        public virtual Person Person { get; set; }

        [ForeignKey("RelatedPersonId")]
        public virtual Person RelatedPerson { get; set; }
    }

    public record City : IEntity
    {
       
        public string Name { get; set; }
      
    }


}
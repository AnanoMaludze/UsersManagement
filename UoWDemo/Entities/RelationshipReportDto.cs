using UsersManagement.Enums;

namespace UsersManagement.Entities
{
    public class RelationshipReportDto
    {
        public int PersonId { get; set; }
        public List<RelationshipTypeCountDto> TypeAndCounts { get; set; }
    }

    public class RelationshipTypeCountDto
    {
        public RelationshipType RelationshipType { get; set; }
        public int Count { get; set; }
    }



}

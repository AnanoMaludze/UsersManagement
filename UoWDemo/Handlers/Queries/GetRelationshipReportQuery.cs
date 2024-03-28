using MediatR;
using Microsoft.EntityFrameworkCore;
using UsersManagement.Entities;
using UsersManagement.Repositories;

namespace UsersManagement.Handlers.Queries
{
    public class GetRelationshipReportQuery : IRequest<List<RelationshipReportDto>>
    {
        public int? PersonId { get; set; }


    }

    public class GetRelationshipReportQueryHandler : IRequestHandler<GetRelationshipReportQuery, List<RelationshipReportDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetRelationshipReportQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        
        public async Task<List<RelationshipReportDto>> Handle(GetRelationshipReportQuery request, CancellationToken cancellationToken)
        {
            var relationships = await _unitOfWork.Repository().FindAllAsync<Relationship>(cancellationToken);

            var filteredRelationships = relationships.AsQueryable();

            if (request.PersonId.HasValue)
            {
                filteredRelationships = filteredRelationships.Where(r => r.PersonId == request.PersonId.Value);
            }

            var groupedData = filteredRelationships
                     .GroupBy(r => r.PersonId)
                     .Select(g => new RelationshipReportDto
                     {
                         PersonId = g.Key,
                         TypeAndCounts = g.GroupBy(r => r.RelationshipType)
                                          .Select(subGroup => new RelationshipTypeCountDto
                                          {
                                              RelationshipType = subGroup.Key,
                                              Count = subGroup.Count()
                                          }).ToList()
                     }).ToList();


            return groupedData;
        }

    }
}

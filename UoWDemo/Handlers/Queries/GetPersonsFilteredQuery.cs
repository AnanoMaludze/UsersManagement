namespace UsersManagement.Handlers.Queries
{
    using AutoMapper;
    using ErrorOr;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using UsersManagement.Entities;
    using UsersManagement.Repositories;
    using UsersManagement.Resources;

    public class GetPersonsFilteredQuery : IRequest<ErrorOr<List<PersonResource>>>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PersonalNumber { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;


        public GetPersonsFilteredQuery(string? firstName = null, string? lastName = null, string? personalNumber = null, int pageNumber = 1, int pageSize = 10)
        {
            FirstName = firstName;
            LastName = lastName;
            PersonalNumber = personalNumber;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

    }
    public class GetPersonsFilteredQueryHandler : IRequestHandler<GetPersonsFilteredQuery, ErrorOr<List<PersonResource>>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetPersonsFilteredQueryHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<ErrorOr<List<PersonResource>>> Handle(GetPersonsFilteredQuery request, CancellationToken cancellationToken)
        {
           
            Expression<Func<Person, bool>> filter = x =>
                (string.IsNullOrWhiteSpace(request.FirstName) || EF.Functions.Like(x.Name, $"%{request.FirstName}%")) &&
                (string.IsNullOrWhiteSpace(request.LastName) || EF.Functions.Like(x.Surname, $"%{request.LastName}%")) &&
                (string.IsNullOrWhiteSpace(request.PersonalNumber) || EF.Functions.Like(x.PersonalNumber, $"%{request.PersonalNumber}%"));


            var filteredPersons = await _uow.Repository().FindListAsyncWithIncludes(filter, null, cancellationToken);

             var pagedPersons = filteredPersons
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return _mapper.Map<List<PersonResource>>(pagedPersons);
        }


    }
}

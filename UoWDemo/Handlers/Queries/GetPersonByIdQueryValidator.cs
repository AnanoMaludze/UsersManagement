using FluentValidation;

namespace UsersManagement.Handlers.Queries
{
    public class GetPersonByIdQueryValidator : AbstractValidator<GetPersonByIdQuery>
    {
        public GetPersonByIdQueryValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}

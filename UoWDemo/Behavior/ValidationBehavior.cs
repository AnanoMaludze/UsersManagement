using ErrorOr;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using System.Reflection;

namespace UsersManagement.Behavior
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
       where TRequest : IRequest<TResponse>
       where TResponse : IErrorOr
    {
        private readonly IEnumerable<IValidator<TRequest>> _validator;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validator)
        {
            _validator = validator;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validator.Any())
            {
                return await next();
            }

            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(_validator.Select(v => v.ValidateAsync(context, cancellationToken)));
            var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

            if (failures.Count != 0)
            {
                var errors = failures.Select(failure => Error.Validation(failure.PropertyName, failure.ErrorMessage)).ToList();
                var responseType = typeof(TResponse);

                if (responseType.GetMethod(nameof(ErrorOr<object>.From))?.Invoke(null, new object[] { errors }) is TResponse response)
                {
                    return response;
                }

                throw new ValidationException(failures);
            }

            return await next();
        }

        private static bool TryCreateResponseFromErrors(List<ValidationFailure> validationFailures, out TResponse response)
        {
            List<Error> errors = validationFailures.ConvertAll(x => Error.Validation(
                code: x.PropertyName,
                description: x.ErrorMessage));

            response = (TResponse?)typeof(TResponse)
                .GetMethod(
                    name: nameof(ErrorOr<object>.From),
                    bindingAttr: BindingFlags.Static | BindingFlags.Public,
                    types: new[] { typeof(List<Error>) })?
                .Invoke(null, new[] { errors })!;

            return response is not null;
        }
    }
}

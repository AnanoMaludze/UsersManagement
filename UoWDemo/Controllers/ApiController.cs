using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace UsersManagement.Controllers
{
    public class ApiController : ControllerBase
    {
        protected IActionResult Problem(List<ErrorOr.Error> errors)
        {
            if (errors.Count is 0)
                return Problem();

            if (errors.All(e => e.Type == ErrorType.Validation))
            {
                var modelStateDict = new ModelStateDictionary();
                foreach (var error in errors)
                {
                    modelStateDict.AddModelError(error.Code, error.Description);
                }
                return ValidationProblem(modelStateDict);
            }

            HttpContext.Items["errors"] = errors;

            var firstError = errors[0];
            var statusCode = firstError.Type switch
            {
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError
            };

            return Problem(statusCode: statusCode, title: firstError.Description);
        }
    }
}

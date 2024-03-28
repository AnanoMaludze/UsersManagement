using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using UsersManagement.Handlers.Commands;
using UsersManagement.Handlers.Queries;
using UsersManagement.Resources;

namespace UsersManagement.Controllers
{
    [Route("v1/api/[controller]")]
    [ApiController]
    public class PersonsController : ApiController
    {
        private readonly ISender _mediator;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PersonsController(ISender mediator, IWebHostEnvironment webHostEnvironment)
        {
            _mediator = mediator;
            _webHostEnvironment = webHostEnvironment;

        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        [ProducesErrorResponseType(typeof(ErrorOr.ErrorOr))]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetPersonByIdQuery(id);
            var result = await _mediator.Send(query);

            return result.Match(resp => StatusCode((int)HttpStatusCode.OK, resp),
                errors => Problem(errors));
        }

        [HttpPost(nameof(Add))]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        [ProducesErrorResponseType(typeof(ErrorOr.ErrorOr))]
        public async Task<IActionResult> Add(CreatePersonCommand request)
        {

            var result = await _mediator.Send(request);

            return result.Match(resp => StatusCode((int)HttpStatusCode.OK, resp),
                errors => Problem(errors));
        }

        [HttpPut("Update{id}")]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        [ProducesErrorResponseType(typeof(ErrorOr.ErrorOr))]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePersonCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("Mismatched person ID");
            }

            var result = await _mediator.Send(command);

            return result.Match(
                personResource => Ok(personResource),
                errors => Problem(errors));
        }


        [HttpPost("{personId}/upload-image")]
        public async Task<IActionResult> UploadImage(int personId, IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return BadRequest("No image file provided.");
            }

            var command = new UploadOrChangeImageCommand(personId, imageFile);
            var result = await _mediator.Send(command);

            return result.Match(
                imagePath => Ok(new { ImagePath = imagePath }),
                errors => Problem(errors));
        }
        [HttpDelete("{personId}")]
        public async Task<IActionResult> DeletePerson(int personId)
        {
            var command = new DeletePersonCommand(personId);
            var result = await _mediator.Send(command);

            return result.Match(
                success => Ok(),
                errors => Problem(errors));
        }

        [HttpDelete("relationships/{relationshipId}")]
        public async Task<IActionResult> DeleteRelationship(int relationshipId)
        {
            var command = new DeleteRelationshipCommand(relationshipId);
            var result = await _mediator.Send(command);

            return result.Match(
                success => Ok(),
                errors => Problem(errors));
        }


        [HttpPost("{personId}/relationships")]
        public async Task<IActionResult> AddRelationship(int personId, [FromBody] AddRelationshipCommand request)
        {
            if (request.RelatedPersonId == 0 || personId == request.RelatedPersonId)
            {
                return BadRequest("Invalid related person ID.");
            }

            var command = new AddRelationshipCommand(personId, request.RelatedPersonId, request.RelationshipType);
            var result = await _mediator.Send(command);

            return result.Match(
                success => Ok(),
                errors => Problem(errors));
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<PersonResource>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetFilteredPersons(
            [FromQuery] string? firstName = null,  
            [FromQuery] string? lastName = null,  
            [FromQuery] string? personalNumber = null, 
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
                {
            var query = new GetPersonsFilteredQuery(firstName, lastName, personalNumber, pageNumber, pageSize);
            var result = await _mediator.Send(query);

            return result.Match(
                persons => Ok(persons),
                errors => Problem(errors));
        }

        [HttpGet("relationship-report")]
        public async Task<IActionResult> GetRelationshipReport([FromQuery] GetRelationshipReportQuery request)
        {
            var result = await _mediator.Send(request);

            if (!result.Any())
            {
                return NotFound("No relationships was found.");
            }

            return Ok(result);
        }


    }
}

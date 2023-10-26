using Microsoft.AspNetCore.Mvc;
using Organization.Services;

namespace Organization.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        private ILogger<OrganizationController> _logger;
        private IOrganizationRepository _repository;

        public OrganizationController(ILogger<OrganizationController> logger, IOrganizationRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        // POST api/<OrganizationController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Models.Organization organization)
        {
            string? organizationId = await _repository.Create(organization);

            if (organizationId is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "failed to create organization");
            }

            return Ok(organizationId);
        }
    }
}

using Microsoft.AspNetCore.Http;
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
        
        // GET: api/<OrganizationController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<OrganizationController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<OrganizationController>
        [HttpPost]
        public IActionResult Post([FromBody] Models.Organization organization)
        {
            string? organizationId = _repository.Create(organization);

            if (organizationId is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "failed to create organization");
            }

            return Ok(organizationId);
        }

        // PUT api/<OrganizationController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<OrganizationController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

using Cuplan.Organization.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cuplan.Organization.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrganizationController(ILogger<OrganizationController> logger, IOrganizationRepository repository)
    : ControllerBase
{
    // POST api/<OrganizationController>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Post([FromBody] Models.Organization organization)
    {
        string? organizationId = await repository.Create(organization);

        if (organizationId is null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "failed to create organization");
        }

        return Ok(organizationId);
    }
}


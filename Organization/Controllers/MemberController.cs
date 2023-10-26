using Cuplan.Organization.Models;
using Cuplan.Organization.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cuplan.Organization.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MemberController(MemberManager memberManager) : ControllerBase
{
    // POST api/<MemberController>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Post([FromBody] Member member)
    {
        string? memberId = await memberManager.Create(member);

        if (memberId is null)
        {
            return BadRequest("org id not found");
        }
        
        return Ok("");
    }
}
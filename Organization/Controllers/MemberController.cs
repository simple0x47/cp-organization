using Core;
using Cuplan.Organization.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Organization;

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
        Result<string, Error<ErrorKind>> createMemberResult = await memberManager.Create(member);

        if (!createMemberResult.IsOk)
        {
            Error<ErrorKind> error = createMemberResult.UnwrapErr();

            if (error.ErrorKind == ErrorKind.NotFound) return BadRequest("org id not found");

            return StatusCode(StatusCodes.Status500InternalServerError, error.Message);
        }

        return Ok(createMemberResult.Unwrap());
    }
}
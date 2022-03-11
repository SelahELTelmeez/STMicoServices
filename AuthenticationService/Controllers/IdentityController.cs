using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers;

[ApiController, Route("api/[controller]")]
public class IdentityController : ControllerBase
{
    public IdentityController()
    {

    }

    [HttpPost("[action]")]
    public IActionResult Login()
    {
        return Ok();
    }
    [HttpPost("[action]")]
    public IActionResult Register()
    {
        return Ok();
    }
    [HttpPost("[action]")]
    public IActionResult RefreshToken()
    {
        return Ok();
    }
}
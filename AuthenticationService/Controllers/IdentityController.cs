using IdentityEntities.Entities;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers;

[ApiController, Route("api/[controller]")]
public class IdentityController : ControllerBase
{
    public IdentityController()
    {

    }

    [HttpPost]
    public IActionResult Login()
    {
        return Ok();
    }

    public IActionResult Register()
    {
        return Ok();
    }

    [HttpPost]
    public IActionResult RefreshToken()
    {
        return Ok();
    }
}
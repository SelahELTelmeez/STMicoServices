using Microsoft.AspNetCore.Mvc;

namespace AuthenticationService.Controllers
{
    [ApiController, Route("api/[controller]")]

    public class AuthController : ControllerBase
    {
        public AuthController()
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
}

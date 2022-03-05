using CurriculumEntites.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CurriculumService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase    /////////   For Test This Controller
    {
        CurriculumDbContext _curriculumDbContext;
        public ValuesController(CurriculumDbContext curriculumDbContext)
        {
            _curriculumDbContext = curriculumDbContext;

            _curriculumDbContext.Database.EnsureCreated();
        }

        [HttpGet("GetBasketId")]
        public IActionResult GetBasketId()
            => Ok();
    }
}

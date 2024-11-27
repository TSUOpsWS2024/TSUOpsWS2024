using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Models;
using Web.Api.Data;
using Web.Api.Services;
using System.IO;

namespace Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeneratedImagesController : ControllerBase
    {
        private readonly ApiContext _context;

        public GeneratedImagesController(ApiContext context)
        {
            _context = context;
        }

        //Get
        [HttpGet]
        public async Task<IActionResult> Download()
        {
            var filestream = GeneratedImagesService.Work();
            if (filestream != null)
            {
                return File(filestream, "application/octet-stream", "Result.png");
            }
            return new JsonResult(NotFound());

        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Models;
using Web.Api.Data;

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

        //Create/Edit
        [HttpPost]
        public JsonResult CreateEdit(GeneratedImage image)
        {
            if (image.Id == 0)
            {
                _context.GeneratedImages.Add(image);
            } else
            {
                var imageInDb = _context.GeneratedImages.Find(image.Id);

                if (imageInDb == null)
                    return new JsonResult(NotFound());

                imageInDb = image;
            }

            _context.SaveChanges();
            return new JsonResult(Ok(image));
        }

        //Get
        [HttpGet]
        public JsonResult Get(int id)
        {
            var result = _context.GeneratedImages.Find(id);
            if (result == null) 
                return new JsonResult(NotFound());
            return new JsonResult(Ok(result));
        }

        //Delete
        [HttpDelete]
        public JsonResult Delete(int id)
        {
            var result = _context.GeneratedImages.Find(id);
            if (result == null)
                return new JsonResult(NotFound());

            _context.GeneratedImages.Remove(result);
            _context.SaveChanges();
            return new JsonResult(NoContent());
        }
    }
}

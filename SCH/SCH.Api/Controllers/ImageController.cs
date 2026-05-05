namespace SCH.API.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using SCH.Services.Images;
    using SCH.Services.Students;
    using SCH.Shared.Exceptions;
    using static System.Net.Mime.MediaTypeNames;
    using System.Drawing;
    using SCH.Models.Image;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ImageController : Controller
    {
        private readonly IImageService imageService;

        public ImageController(IImageService imageService) 
        {
            this.imageService = imageService;
        }

        [HttpGet("getStudentProfile/{imageName}")]
        public IActionResult GetStudentProfile(string imageName)
        {
            IActionResult actionResult;
            if (string.IsNullOrWhiteSpace(imageName))
            {
                actionResult = BadRequest("file is not set");
            }

            ImageFileDto imagefile = imageService
                .GetStudentProfile(imageName);

            actionResult = File(
                imagefile.FileStream, imagefile.ContentType);

            return actionResult;
        }





        [HttpPost("uploadStudentProfile")]
        public async Task<IActionResult> PostStudentProfileAsync(IFormFile file)
        {
            IActionResult actionResult;

            if (file?.Length > 0)
            {
                string filename = await imageService.UploadStudentProfileAsync(file);

                object obj = new { filename };
                actionResult = Ok(obj);
            }
            else
            {
                actionResult = BadRequest("No file uploaded.");
            }


            return actionResult;
        }

        [HttpDelete("deleteStudentProfile/{fileName}")]
        public IActionResult DeleteStudentProfile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw SCHDomainException.BadRequest("Id should grater than 0");
            }

            imageService.DeleteStudentProfile(fileName);

            return Ok();
        }
    }
}

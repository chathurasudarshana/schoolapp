using Microsoft.AspNetCore.Http;
using SCH.Models.Image;
namespace SCH.Services.Images
{
    public interface IImageService : IService
    {
        ImageFileDto GetStudentProfile(string imageName);

        Task<string> UploadStudentProfileAsync(IFormFile file);

        void DeleteStudentProfile(string fileName);

    }
}

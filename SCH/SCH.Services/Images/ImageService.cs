namespace SCH.Services.Images
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using SCH.Models.Image;
    using SCH.Shared.Exceptions;
    using System;

    internal class ImageService : IImageService
    {
        private readonly IConfiguration configuration;

        public ImageService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public ImageFileDto GetStudentProfile(string imageName)
        {
            string? studentImageFolder = configuration["AppSettings:StudentImageFolder"];

            if (string.IsNullOrWhiteSpace(studentImageFolder))
            {
                throw SCHApplicationException.InternalServerError("Student image folder not set.");
            }

            ImageFileDto imageFile = this.GetImage(studentImageFolder, imageName);
            return imageFile;
        }



        private ImageFileDto GetImage(string folder, string imageName)
        {

            string? imageFolder = configuration["AppSettings:ImageFolder"];

            if (string.IsNullOrWhiteSpace(imageFolder))
            {
                throw SCHApplicationException.InternalServerError("Image folder not set.");
            }

            string folderPath = Path.Combine(imageFolder, folder);

            string imagePath = Path.Combine(folderPath, imageName);


            if (!File.Exists(imagePath))
            {
                throw SCHDomainException.NotFound("Image not found");
            }

            FileStream fileStream = new FileStream(
                imagePath, FileMode.Open, FileAccess.Read);

            string fileExtension = Path.GetExtension(imageName).ToLower();

            string contentType = fileExtension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };

            ImageFileDto imageFile = new ImageFileDto
            {
                FileStream = fileStream,
                ContentType = contentType
            };

            return imageFile;
        }




        public async Task<string> UploadStudentProfileAsync(IFormFile file)
        {
            string? studentImageFolder = configuration["AppSettings:StudentImageFolder"];

            if (string.IsNullOrWhiteSpace(studentImageFolder))
            {
                throw SCHApplicationException.InternalServerError("Student image folder not set.");
            }

            return await UploadAsync(file, studentImageFolder);

        }

        private async Task<string> UploadAsync(IFormFile file, string folder)
        {
            string? allowTypes = configuration["AppSettings:AllowImageExtensions"];

            if (string.IsNullOrWhiteSpace(allowTypes))
            {
                throw SCHApplicationException.InternalServerError("Image extensions not set.");
            }

            string[] allowedExtensions = allowTypes.Split(',');

            string fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
            {
                throw SCHDomainException.BadRequest("Invalid file type. Only image files are allowed.");
            }

            string fileName = Path.GetRandomFileName() + fileExtension;

            string? imageFolder = configuration["AppSettings:ImageFolder"];

            if (string.IsNullOrWhiteSpace(imageFolder))
            {
                throw SCHApplicationException.InternalServerError("Image folder not set.");
            }

            string folderPath = Path.Combine(imageFolder, folder);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string filePath = Path.Combine(folderPath, fileName);

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return fileName;
        }

        public void DeleteStudentProfile(string fileName)
        {
            string? studentImageFolder = configuration["AppSettings:StudentImageFolder"];

            if (string.IsNullOrWhiteSpace(studentImageFolder))
            {
                throw SCHApplicationException.InternalServerError("Student image folder not set.");
            }

            Delete(fileName, studentImageFolder);

        }

        private void Delete(string fileName, string folder)
        {
            string? imageFolder = configuration["AppSettings:ImageFolder"];

            if (string.IsNullOrWhiteSpace(imageFolder))
            {
                throw SCHApplicationException.InternalServerError("Image folder not set.");
            }

            string folderPath = Path.Combine(imageFolder, folder);
            string filePath = Path.Combine(folderPath, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

        }

    }
}

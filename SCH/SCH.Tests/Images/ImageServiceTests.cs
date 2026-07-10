namespace SCH.Tests.Images
{
    using Microsoft.Extensions.Configuration;
    using Moq;
    using SCH.Services.Images;
    using SCH.Shared.Exceptions;
    using Xunit;
    using Microsoft.AspNetCore.Http;

    public class ImageServiceTests
    {
        private readonly Mock<IConfiguration> _configuration = new();
        private readonly ImageService _sut;

        public ImageServiceTests()
        {
            _sut = new ImageService(_configuration.Object);
        }

        [Fact]
        public void GetStudentProfile_WhenStudentImageFolderMissing_ThrowsInternalServerError()
        {
            _configuration.Setup(c => c["AppSettings:StudentImageFolder"]).Returns((string?)null);

            Assert.Throws<SCHApplicationException>(() => _sut.GetStudentProfile("test.jpg"));
        }

        [Fact]
        public void GetStudentProfile_WhenImageFolderMissing_ThrowsInternalServerError()
        {
            _configuration.Setup(c => c["AppSettings:StudentImageFolder"]).Returns("students");
            _configuration.Setup(c => c["AppSettings:ImageFolder"]).Returns((string?)null);

            Assert.Throws<SCHApplicationException>(() => _sut.GetStudentProfile("test.jpg"));
        }

        [Fact]
        public async Task UploadStudentProfileAsync_WhenStudentImageFolderMissing_ThrowsInternalServerError()
        {
            _configuration.Setup(c => c["AppSettings:StudentImageFolder"]).Returns((string?)null);
            var file = new Mock<IFormFile>();

            await Assert.ThrowsAsync<SCHApplicationException>(
                () => _sut.UploadStudentProfileAsync(file.Object));
        }

        [Fact]
        public async Task UploadStudentProfileAsync_WhenAllowImageExtensionsMissing_ThrowsInternalServerError()
        {
            _configuration.Setup(c => c["AppSettings:StudentImageFolder"]).Returns("students");
            _configuration.Setup(c => c["AppSettings:AllowImageExtensions"]).Returns((string?)null);
            var file = new Mock<IFormFile>();

            await Assert.ThrowsAsync<SCHApplicationException>(
                () => _sut.UploadStudentProfileAsync(file.Object));
        }

        [Fact]
        public async Task UploadStudentProfileAsync_WhenInvalidExtension_ThrowsBadRequest()
        {
            _configuration.Setup(c => c["AppSettings:StudentImageFolder"]).Returns("students");
            _configuration.Setup(c => c["AppSettings:AllowImageExtensions"]).Returns(".jpg,.png,.gif");
            var file = new Mock<IFormFile>();
            file.Setup(f => f.FileName).Returns("malware.exe");

            await Assert.ThrowsAsync<SCHDomainException>(
                () => _sut.UploadStudentProfileAsync(file.Object));
        }

        [Fact]
        public void DeleteStudentProfile_WhenStudentImageFolderMissing_ThrowsInternalServerError()
        {
            _configuration.Setup(c => c["AppSettings:StudentImageFolder"]).Returns((string?)null);

            Assert.Throws<SCHApplicationException>(() => _sut.DeleteStudentProfile("test.jpg"));
        }
    }
}

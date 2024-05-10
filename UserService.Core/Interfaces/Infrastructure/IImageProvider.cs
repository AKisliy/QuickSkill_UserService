using Microsoft.AspNetCore.Http;

namespace UserService.Core.Interfaces.Infrastructure
{
    public interface IImageProvider
    {
        public Task<string?> UploadFileAsync(IFormFile file, int userId);

        public Task LoadSamplesAvatarsAsync();

        public string? GetRandomSampleImage();
    }
}
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using UserService.Core.Interfaces.Infrastructure;
using UserService.Infrastructure.Options;
using YandexDisk.Client.Http;

namespace UserService.Infrastructure
{
    public class ImageProvider: IImageProvider
    {
        private readonly HttpClient httpClient;

        private readonly YandexDiskOptions _options;

        public ImageProvider(IOptions<YandexDiskOptions> options)
        {
            _options = options.Value;
            httpClient = new HttpClient();
            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", _options.Key);
        }

        public async Task<string?> UploadFileAsync(IFormFile file, int userId)
        {
            var api = new DiskHttpApi(_options.Key);

            var path = GetPathForUser(file, userId);

            var link = await api.Files.GetUploadLinkAsync(path, overwrite: true);

            // Загружаем файл по полученному URL
            using (var fileStream = file.OpenReadStream())
            using (var content = new StreamContent(fileStream))
            {
                await api.Files.UploadAsync(link, fileStream);
            }

            // Возвращаем ссылку для просмотра
            return await PublishFileAsync(path);
        }

        public async Task<string?> PublishFileAsync(string path)
        {
            var publishResponse = await httpClient.PutAsync($"https://cloud-api.yandex.net/v1/disk/resources/publish?path={path}", null);
            publishResponse.EnsureSuccessStatusCode();

            var publicUrlResponse = await httpClient.GetAsync($"https://cloud-api.yandex.net/v1/disk/resources?path={path}&fields=public_url");
            publicUrlResponse.EnsureSuccessStatusCode();

            var filePublicLink = await JsonSerializer.DeserializeAsync<FilePublicLink>(await publicUrlResponse.Content.ReadAsStreamAsync());
            return filePublicLink?.PublicUrl;
        }

        private string GetPathForUser(IFormFile file, int userId)
        {
            string fileName = Path.GetFileName(file.FileName);
            string extension = Path.GetExtension(fileName);
            string uniqueFileName = $"user_{userId}{extension}";

            string path = $"{_options.BasePath}/{uniqueFileName}";
            return path;
        }

        private class FilePublicLink
        {
            [JsonPropertyName("public_url")]
            public string PublicUrl { get; set; } = null!;
        }
    }
}
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
        private static List<string>? sampleUrls;

        private readonly HttpClient httpClient;

        private readonly YandexDiskOptions _options;

        private const string prefixForLink = "https://getfile.dokpub.com/yandex/get/";

        public ImageProvider(IOptions<YandexDiskOptions> options)
        {
            _options = options.Value;
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", _options.Key);
        }

        public async Task<string?> UploadFileAsync(IFormFile file, int userId)
        {
            var api = new DiskHttpApi(_options.Key);

            var path = GetPathForUser(file, userId);

            var link = await api.Files.GetUploadLinkAsync(path, overwrite: true);

            using (var fileStream = file.OpenReadStream())
            using (var content = new StreamContent(fileStream))
            {
                await api.Files.UploadAsync(link, fileStream);
            }

            return await PublishFileAsync(path);
        }

        public async Task<string?> PublishFileAsync(string path)
        {
            var publishResponse = await httpClient.PutAsync($"https://cloud-api.yandex.net/v1/disk/resources/publish?path={path}", null);
            publishResponse.EnsureSuccessStatusCode();

            var publicUrlResponse = await httpClient.GetAsync($"https://cloud-api.yandex.net/v1/disk/resources?path={path}&fields=public_url");
            publicUrlResponse.EnsureSuccessStatusCode();

            var filePublicLink = await JsonSerializer.DeserializeAsync<Item>(await publicUrlResponse.Content.ReadAsStreamAsync());
            return prefixForLink + filePublicLink?.PublicUrl;
        }

        private string GetPathForUser(IFormFile file, int userId)
        {
            string fileName = Path.GetFileName(file.FileName);
            string extension = Path.GetExtension(fileName);
            string uniqueFileName = $"user_{userId}{extension}";

            string path = $"{_options.BasePath}/{uniqueFileName}";
            return path;
        }

        public async Task LoadSamplesAvatarsAsync()
        {
            var api = new DiskHttpApi(_options.Key);
            var response = await httpClient.GetAsync("https://cloud-api.yandex.net/v1/disk/resources?path=SampleAvatars");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<YandexDiskResponse>(content);

            var urls = data?.Embedded?.Items?
                   .Where(item => !string.IsNullOrEmpty(item.PublicUrl))
                   .Select(item => prefixForLink + item.PublicUrl)
                   .ToList();

            sampleUrls = urls;
        }

        public string? GetRandomSampleImage()
        {
            if (sampleUrls == null || sampleUrls.Count == 0)
                return null;

            var randomIndex = new Random().Next(sampleUrls.Count);
            return sampleUrls[randomIndex];
        }

        public class YandexDiskResponse
        {
            [JsonPropertyName("_embedded")]
            public Embedded? Embedded { get; set; }
        }

        public class Embedded
        {
            [JsonPropertyName("items")]
            public List<Item>? Items { get; set; }
        }

        public class Item
        {

            [JsonPropertyName("public_url")]
            public string? PublicUrl { get; set; }
        }
    }
}
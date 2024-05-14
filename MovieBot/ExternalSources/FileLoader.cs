using System.Net;
using System.Text;
using System.Net.Http.Headers;

namespace MovieBot.ExternalSources;

public class FileLoader
{
    private readonly HttpClient _client;
    
    public FileLoader(HttpClient client)
    {
        _client = client;
    }
    
    public async Task<string> UploadFile(string serverUrl, string file, string fileExtension)
    {
        var data = GetBytes(file);
        var content = new ByteArrayContent(data);
        content.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
        
        var requestContent = new MultipartFormDataContent();
        requestContent.Add(content, "file", $"file.{fileExtension}");

        var response = await _client.PostAsync(serverUrl, requestContent);
        return Encoding.Default.GetString(await response.Content.ReadAsByteArrayAsync());
    }
    
    [Obsolete("Obsolete")]
    private static byte[] GetBytes(string fileUrl)
    {
        using var webClient = new WebClient();
        return webClient.DownloadData(fileUrl);
    }
}

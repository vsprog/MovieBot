using System.Net.Http.Headers;

namespace MovieBot.ExternalSources;

public class FileLoader
{
    private readonly HttpClient _client;
    
    public FileLoader(HttpClient client)
    {
        _client = client;
    }
    
    public async Task<string> UploadFile(string serverUrl, string fileUrl, string fileExtension, CancellationToken cancellationToken)
    {
        var data = await _client.GetByteArrayAsync(fileUrl, cancellationToken);
        var content = new ByteArrayContent(data);
        content.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
        
        var requestContent = new MultipartFormDataContent();
        requestContent.Add(content, "file", $"file.{fileExtension}");

        var response = await _client.PostAsync(serverUrl, requestContent, cancellationToken);
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }
}

using System.Net;
using MovieBot.ExternalSources.MovieLab.Models;

namespace MovieBot.ExternalSources.MovieLab;

public class MovieLabClient
{
    private readonly HttpClient _client;
    private const string Url = "/api/v1/search/movies?page=1&limit=100&title=";

    public MovieLabClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<IEnumerable<LabFilm?>> GetFilms(string title, CancellationToken cancellationToken)
    {
        var response = await _client.GetAsync($"{Url}{title}", cancellationToken);

        if (response.StatusCode != HttpStatusCode.OK)
        {
            return Enumerable.Empty<LabFilm>();
        }
        
        var content = await response.Content.ReadFromJsonAsync<MovieResponse>(cancellationToken: cancellationToken);
        return content!.Films;
    }
}

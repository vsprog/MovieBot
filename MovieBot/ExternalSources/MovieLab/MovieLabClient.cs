using System.Net;
using MovieBot.ExternalSources.MovieLab.Models;
using Newtonsoft.Json;

namespace MovieBot.ExternalSources.MovieLab;

public class MovieLabClient
{
    private readonly HttpClient _client;
    private const string Url = "/api/v1/search/movies?page=1&limit=100&title=";

    public MovieLabClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<IEnumerable<LabFilm?>> GetFilms(string title)
    {
        var response = await _client.GetAsync($"{Url}{title}");

        if (response.StatusCode != HttpStatusCode.OK)
        {
            return [];
        }
        
        var content = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<MovieResponse>(content);
        
        return data!.Films;
    }
}

using System.Net;
using MovieBot.ExternalSources.MovieLab.Models;
using Newtonsoft.Json;

namespace MovieBot.ExternalSources.MovieLab;

public class MovieLabClient
{
    private readonly HttpClient _client;
    private const string Url = "/api/v1/new-search/movies?page=1&limit=100&title=";

    public MovieLabClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<IEnumerable<LabFilm?>> GetFilms(string title, CancellationToken cancellationToken)
    {
        var response = await _client.GetAsync($"{Url}{title}", cancellationToken);

        if (response.StatusCode != HttpStatusCode.OK)
        {
            //return Enumerable.Empty<LabFilm>();
            return new[]
            {
                new LabFilm
                {
                    Player = new LabPlayer
                    {
                        Url = "",
                        Translate = ""
                    },
                    TitleEn = response.StatusCode.ToString(),
                    PosterUrl = response.RequestMessage?.ToString() ?? ""
                }
            };
        }
        
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var data = JsonConvert.DeserializeObject<MovieResponse>(content);
        
        return data!.Films;
    }
}

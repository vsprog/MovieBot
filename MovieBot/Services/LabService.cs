using MovieBot.ExternalSources.MovieLab;
using MovieBot.Services.Models;

namespace MovieBot.Services;

public class LabService
{
    private readonly MovieLabClient _movieLabClient;

    public LabService(MovieLabClient movieLabClient)
    {
        _movieLabClient = movieLabClient;
    }

    public async Task<List<FilmDto>> GetMovies(string query)
    {
        var films = await _movieLabClient.GetFilms(query);

        return films
            .Select(f => new FilmDto(f!))
            .ToList();
    }
}

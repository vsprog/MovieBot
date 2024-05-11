using MovieBot.ExternalSources.Kinopoisk;
using MovieBot.ExternalSources.Yohoho;
using MovieBot.Services.Models;

namespace MovieBot.Services;

public class YohohoService
{
    private readonly KinopoiskClient _kinopoiskClient;
    private readonly YohohoClient _yohohoClient;

    public YohohoService(KinopoiskClient kinopoiskClient, YohohoClient yohohoClient)
    {
        _kinopoiskClient = kinopoiskClient;
        _yohohoClient = yohohoClient;
    }
    
    public async Task<List<FilmDto>?> GetMovies(string query)
    {
        var movieId = await _kinopoiskClient.Search(query);

        if (movieId == null)
        {
            return Enumerable.Empty<FilmDto>().ToList();;
        }
        
        var frames = await _yohohoClient.GetFrames(movieId);
        
        return frames
            .Select(f => new FilmDto(f!, string.Empty))
            .ToList();
    }
}

using MovieBot.ExternalSources.MovieLab.Models;
using MovieBot.ExternalSources.Yohoho.Models;

namespace MovieBot.Services.Models;

public class FilmDto
{
    public string Url { get; init; }
    public string Iframe => $"<iframe width=\"560\" height=\"315\" src=\"{Url}\" frameborder=\"0\" allowfullscreen></iframe>";
    public string Translate { get; init; }
    public string? Title { get; init; }
    public string? PosterLink { get; init; }

    public FilmDto(Frame frame, string? title)
    {
        Url = frame.Url;
        Translate = frame.Translate;
        Title = title;
    }
    
    public FilmDto(LabFilm film)
    {
        Url = film.Player.Url;
        Translate = film.Player.Translate;
        Title = string.IsNullOrEmpty(film.TitleRu) ? film.TitleEn : film.TitleRu;
        PosterLink = film.PosterUrl;
    }
}

using System.Net;
using System.Reflection;
using System.Text;

using MovieBot.Models;

using Newtonsoft.Json;

namespace MovieBot.Services
{
    public class MovieMaker
    {
        private readonly HttpClient client;
        private readonly IEnumerable<PropertyInfo> filmResponseProps;

        public MovieMaker(HttpClient client)
        {
            this.client = client;
            filmResponseProps = typeof(FilmResponse).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }

        public async Task<IEnumerable<Frame>> GetFrames(string movieId)
        {
            var body = JsonConvert.SerializeObject(new
            {
                kinopoisk = movieId,
                tv = 1,
                resize = 1,
                button_limit = 8,
                button_size = 1,
                separator = ",",
                button = "videocdn: {Q} {T}, hdvb: {Q} {T}, bazon: {Q} {T}, ustore: {Q} {T}, alloha: {Q} {T}, kodik: {Q} {T}, iframe: {Q} {T}, collaps: {Q} {T}",
                player = "videocdn,hdvb,bazon,ustore,alloha,kodik,iframe,collaps,pleer",

            });
            HttpResponseMessage response = await client.PostAsync(string.Empty, new StringContent(body, Encoding.UTF8, "application/json"));

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return Enumerable.Empty<Frame>();
            }

            var content = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<FilmResponse>(content);
            var result = filmResponseProps.Select(p => p.GetValue(data) as Frame).Where(f => !string.IsNullOrEmpty(f?.Url));

            return result;
        }
    }
}

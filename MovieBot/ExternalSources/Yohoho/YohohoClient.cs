using System.Net;
using System.Reflection;
using System.Text;
using MovieBot.ExternalSources.Yohoho.Models;
using Newtonsoft.Json;

namespace MovieBot.ExternalSources.Yohoho
{
    public class YohohoClient
    {
        private readonly HttpClient _client;
        private readonly IEnumerable<PropertyInfo> _filmResponseProps;

        public YohohoClient(HttpClient client)
        {
            _client = client;
            _filmResponseProps = typeof(YohohoResponse).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }

        public async Task<IEnumerable<Frame?>> GetFrames(string movieId)
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
            var response = await _client.PostAsync(string.Empty, new StringContent(body, Encoding.UTF8, "application/json"));

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return Enumerable.Empty<Frame>();;
            }

            var content = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<YohohoResponse>(content);
            var result = _filmResponseProps
                .Select(p => p.GetValue(data) as Frame)
                .Where(f => !string.IsNullOrEmpty(f?.Url));

            return result;
        }
    }
}

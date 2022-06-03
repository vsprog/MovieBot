using System.Net;
using System.Text;

using MovieBot.Models;

using Newtonsoft.Json;

namespace MovieBot.Services
{
    public class MovieFinder
    {
        private readonly HttpClient client;

        public MovieFinder(HttpClient client)
        {
            this.client = client;
        }

        public async Task<string?> Search(string queryString)
        {
            string? token = await GetToken();

            if (string.IsNullOrEmpty(queryString) || string.IsNullOrEmpty(token))
            {
                return null;
            }

            client.DefaultRequestHeaders.Add("x-token", token);
            client.DefaultRequestHeaders.Add("service-id", "25");
            var body = JsonConvert.SerializeObject(new
            {
                query = "query SuggestSearch($keyword: String!, $yandexCityId: Int, $limit: Int) { suggest(keyword: $keyword) { top(yandexCityId: $yandexCityId, limit: $limit) { topResult { global { ...SuggestMovieItem ...SuggestPersonItem ...SuggestCinemaItem ...SuggestMovieListItem __typename } __typename } movies { movie { ...SuggestMovieItem __typename } __typename } persons { person { ...SuggestPersonItem __typename } __typename } cinemas { cinema { ...SuggestCinemaItem __typename } __typename } movieLists { movieList { ...SuggestMovieListItem __typename } __typename } __typename } __typename } } fragment SuggestMovieItem on Movie { id title { russian original __typename } rating { kinopoisk { isActive value __typename } __typename } poster { avatarsUrl fallbackUrl __typename } onlineViewOptions { textToDisplay isAvailableOnline isPurchased subscriptionPurchaseTag accessType availabilityAnnounce { groupPeriodType announcePromise availabilityDate type __typename } __typename } ... on Film { type productionYear __typename } ... on TvSeries { releaseYears { end start __typename } __typename } ... on TvShow { releaseYears { end start __typename } __typename } ... on MiniSeries { releaseYears { end start __typename } __typename } __typename } fragment SuggestPersonItem on Person { id name originalName birthDate poster { avatarsUrl fallbackUrl __typename } __typename } fragment SuggestCinemaItem on Cinema { id ctitle: title city { id name geoId __typename } __typename } fragment SuggestMovieListItem on MovieListMeta { id cover { avatarsUrl __typename } coverBackground { avatarsUrl __typename } name url description movies(limit: 0) { total __typename } __typename } ",
                operationName = "SuggestSearch",
                variables = new 
                {
                    keyword = queryString,
                    yandexCityId = 65,
                    limit = 3,
                }
            });

            HttpResponseMessage response = await client.PostAsync("api-frontend//graphql", new StringContent(body, Encoding.UTF8, "application/json"));
            client.DefaultRequestHeaders.Remove("x-token");
            client.DefaultRequestHeaders.Remove("service-id");

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject<dynamic>(content);
            dynamic res = null;
            
            try
            {
                res = data["data"]["suggest"]["top"]["topResult"]["global"]["id"];
            }
            catch
            {
                return null;
            }

            return res;
        }

        private async Task<string?> GetToken()
        {
            client.DefaultRequestHeaders.Add("x-requested-with", "XMLHttpRequest");
            HttpResponseMessage response = await client.GetAsync("/api-frontend/token");
            client.DefaultRequestHeaders.Remove("x-requested-with");

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<TokenResponse>(content);

            return data?.Token;
        }
    }
}

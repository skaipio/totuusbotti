using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RedditSharp;

namespace TotuusBotti
{
    class RedditBot
    {
        private static string accessTokenAddress = "https://www.reddit.com/api/v1/access_token";
        private string clientId;
        private string clientSecret;
        private HttpClient redditHttpClient;
        private Reddit redditClient;
        private WebAgent redditWebAgent;
        private DateTime accessTokenExpires;

        public async void Configure()
        {
            clientId = Environment.GetEnvironmentVariable("REDDITTOTUUSBOTTI_CLIENT_ID");
            clientSecret = Environment.GetEnvironmentVariable("REDDITTOTUUSBOTTI_CLIENT_SECRET");
            redditHttpClient = new HttpClient();
            var redditCredentials = Encoding.ASCII.GetBytes(clientId + ":" + clientSecret);
            var authHeader = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(redditCredentials));
            redditHttpClient.DefaultRequestHeaders.Authorization = authHeader;
            redditWebAgent = new WebAgent();

            if (isAccessTokenExpired())
            {
                await RefreshAccessToken();
            }

            redditClient = new Reddit(redditWebAgent, false);
        }

        public async Task<IEnumerable<RedditSharp.Things.Post>> GetPostsFromSubreddit(string subredditName, int postCount)
        {
            // Figure out how to wrap requests so that we don't have to call this every time
            if (isAccessTokenExpired())
            {
                await RefreshAccessToken();
            }
            var subreddit = redditClient.GetSubreddit(subredditName);
            return subreddit.Posts.Take(postCount);
        }

        public Task<IEnumerable<RedditSharp.Things.Post>> GetPoliticalCompassMemes(int postCount)
        {
            return GetPostsFromSubreddit("/r/politicalcompassmemes", postCount);
        }

        private async Task RefreshAccessToken()
        {
            var keyValues = new List<KeyValuePair<string, string>>();
            keyValues.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
            var requestContent = new FormUrlEncodedContent(keyValues);
            var response = await redditHttpClient.PostAsync(accessTokenAddress, requestContent);
            var content = await response.Content.ReadAsStringAsync();
            dynamic json = JsonConvert.DeserializeObject(content);
            var expiration_time = (int)json.expires_in;
            // Make expiration 5 second earlier to compensate for request lag
            accessTokenExpires = DateTime.Now.AddSeconds(expiration_time - 5);
            redditWebAgent.AccessToken = json.access_token;
        }

        private bool isAccessTokenExpired()
        {
            return DateTime.Now > accessTokenExpires;
        }
    }
}

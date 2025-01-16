using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace fluxPay.Clients
{
    public class KeyCloakClient
    {
        private readonly IConfiguration configuration;
        public HttpClient Client { get; }

        public KeyCloakClient(HttpClient client, IConfiguration configuration)
        {
            this.configuration = configuration;
            Client = client;
            Client.BaseAddress = new Uri(configuration["Keycloak:BaseUrl"]);
            Client.Timeout = new TimeSpan(0, 0, 30);
            Client.DefaultRequestHeaders.Clear();
        }

        public async Task<string> GetAdminTokenAsync()
        {
            var tokenUrl = $"{Client.BaseAddress}/realms/{configuration["Keycloak:Realm"]}/protocol/openid-connect/token";
            var clientId = configuration["Keycloak:ClientId"];
            var clientSecret = configuration["Keycloak:ClientSecret"];
            var clientCredentials = configuration["Keycloak:ClientCredentials"];

            var authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
            var request = new HttpRequestMessage(HttpMethod.Post, tokenUrl)
            {
                Content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", clientCredentials),
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("client_secret", clientSecret)
                })
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authHeaderValue);

            var response = await Client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<dynamic>(content);
                return json.access_token;
            }

            throw new Exception("Failed to obtain admin token.");
        }
    }

    public class KeyCloakTokenHandler : DelegatingHandler
    {
        private readonly string _tokenUrl;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _realm;
        private string _accessToken;
        private DateTime _tokenExpiration;
        private readonly HttpClient _authClient;
        private readonly SemaphoreSlim _tokenSemaphore = new SemaphoreSlim(1, 1);
        private readonly IConfiguration _configuration;

        public KeyCloakTokenHandler(IConfiguration configuration, HttpClient authClient)
        {
            _configuration = configuration;
            _authClient = authClient;
            _tokenUrl = $"{_configuration["Keycloak:BaseUrl"]}/realms/{_configuration["Keycloak:Realm"]}/protocol/openid-connect/token";
            _clientId = _configuration["Keycloak:ClientId"];
            _clientSecret = _configuration["Keycloak:ClientSecret"];
            _realm = _configuration["Keycloak:Realm"];
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_accessToken) || DateTime.UtcNow >= _tokenExpiration)
            {
                await FetchTokenAsync();
            }

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

            var response = await base.SendAsync(request, cancellationToken);

            // If the token has expired, fetch a new one and retry the request
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await FetchTokenAsync();
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
                response = await base.SendAsync(request, cancellationToken);
            }

            return response;
        }

        private async Task FetchTokenAsync()
        {
            await _tokenSemaphore.WaitAsync();
            try
            {
                if (string.IsNullOrEmpty(_accessToken) || DateTime.UtcNow >= _tokenExpiration)
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, _tokenUrl)
                    {
                        Content = new FormUrlEncodedContent(new[]
                        {
                            new KeyValuePair<string, string>("grant_type", "client_credentials"),
                            new KeyValuePair<string, string>("client_id", _clientId),
                            new KeyValuePair<string, string>("client_secret", _clientSecret)
                        })
                    };

                    var response = await _authClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync();
                    var tokenResponse = JsonConvert.DeserializeObject<dynamic>(content);

                    _accessToken = tokenResponse.access_token;
                    _tokenExpiration = DateTime.UtcNow.AddSeconds((int)tokenResponse.expires_in - 60); // Subtract 60 seconds for buffer
                }
            }
            finally
            {
                _tokenSemaphore.Release();
            }
        }
    }
}

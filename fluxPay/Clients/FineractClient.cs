using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace fluxPay.Clients
{
    public class FineractClient
    {
        public readonly HttpClient _client;
        private readonly IConfiguration _configuration;
        private readonly string _tenantId;

        public FineractClient(HttpClient client, IConfiguration configuration, string tenantId = "default")
        {
            _client = client;
            _tenantId = tenantId;
            _configuration = configuration;

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            // Recreate HttpClient with the custom handler
            _client = new HttpClient(handler)
            {
                BaseAddress = new Uri(_configuration["Fineract:BaseUrl"]),
                Timeout = TimeSpan.FromSeconds(30)
            };

            if (!_client.DefaultRequestHeaders.Contains("Authorization"))
            {
                var authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_configuration["Fineract:Username"]}:{_configuration["Fineract:Password"]}"));
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authValue);
            }

            if (!_client.DefaultRequestHeaders.Contains("Fineract-Platform-TenantId"))
            {
                _client.DefaultRequestHeaders.Add("Fineract-Platform-TenantId", _tenantId);
            }

            // Log the headers for debugging
            Console.WriteLine("Default Request Headers:");
            foreach (var header in _client.DefaultRequestHeaders)
            {
                Console.WriteLine($"{header.Key}: {string.Join(", ", header.Value)}");
            }
            Console.WriteLine($"Tenant ID: {_tenantId}");

            // Disable SSL Verification (for development purposes only)
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((sender, certificate, chain, sslPolicyErrors) => true);
        }

        public async Task<HttpResponseMessage> GetAsync(string endpoint)
        {
            var response = await _client.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return response;
        }

        public async Task<HttpResponseMessage> PostAsync(string endpoint, HttpContent content)
        {
            var response = await _client.PostAsync(endpoint, content);
            response.EnsureSuccessStatusCode();
            return response;
        }
    }
}

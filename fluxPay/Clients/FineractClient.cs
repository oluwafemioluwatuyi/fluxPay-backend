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
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;

        public FineractClient(HttpClient client, IConfiguration configuration)
        {
            _configuration = configuration;
             var handler = new HttpClientHandler 
             { ServerCertificateCustomValidationCallback = (
                message, cert, chain, errors) => true
             }; 
             _client = new HttpClient(handler) { BaseAddress = new Uri(_configuration["Fineract:BaseUrl"]), Timeout = TimeSpan.FromSeconds(30) };

            // Add Basic Authentication Header
            var authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_configuration["Fineract:Username"]}:{_configuration["Fineract:Password"]}"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authValue);

             // Disable SSL Verification (for development purposes only)
          //  ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((sender, certificate, chain, sslPolicyErrors) => true);
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

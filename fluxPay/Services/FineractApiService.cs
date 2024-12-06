using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using fluxPay.Clients;
using fluxPay.DTOs; 
using fluxPay.Interfaces.Services;
using FluxPay.Models;

namespace fluxPay.Services
{
    public class FineractApiService : IFineractApiService
    {
        private readonly FineractClient _fineractClient;


        public FineractApiService(FineractClient fineractClient)
        {
            _fineractClient = fineractClient;
        }

        public async Task<string> CreateClientAsync(CreateClientRequestDto createClientRequestDto)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };

            var jsonContent = System.Text.Json.JsonSerializer.Serialize(createClientRequestDto, options);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _fineractClient.PostAsync("/clients", content);

            if (response.IsSuccessStatusCode)
            {
                var successContent = await response.Content.ReadAsStringAsync();
                return successContent;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error creating client: {response.StatusCode}, {errorContent}");
            }
        }

        public async Task<string> GetClientAsync(int clientId)
        {
            var requestUri = $"/fineract-provider/api/v1/clients/{clientId}";

            Console.WriteLine("Request URI: " + requestUri);
            Console.WriteLine("Authorization Header: " + _fineractClient._client.DefaultRequestHeaders.Authorization);

            var response = await _fineractClient._client.GetAsync(requestUri);

            if (response.IsSuccessStatusCode)
            {
                var successContent = await response.Content.ReadAsStringAsync();
                return successContent;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error retrieving client: {response.StatusCode}, {errorContent}");
            }
        }
    }
}

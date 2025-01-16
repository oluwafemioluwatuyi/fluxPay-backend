using fluxPay.Interfaces.Services;
using fluxPay.DTOs.AuthDtos;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using fluxPay.Helpers;
using fluxPay.Clients;

namespace fluxPay.Services
{
    public class KeyCloak : IKeyCloak
    {
        private readonly KeyCloakClient _keycloakClient;
        private readonly IConfiguration _configuration;

        public KeyCloak(KeyCloakClient keycloakClient, IConfiguration configuration)
        {
            _keycloakClient = keycloakClient;
            _configuration = configuration;
        }

        public async Task CreateUser(RegisterRequestDto1 registerRequestDto1)
        {
            var token = await _keycloakClient.GetAdminTokenAsync();
            var realm = "fluxpay";

            // Step 1: Create the user
            var userPayload = new
            {
                username = registerRequestDto1.UserName,
                email = registerRequestDto1.Email,
                firstName = registerRequestDto1.FirstName,
                lastName = registerRequestDto1.LastName,
                enabled = true
            };

            // Use KeyCloakClient's client to make the request
            var createUserResponse = await _keycloakClient.Client.PostAsync(
                $"/admin/realms/{realm}/users",
                new StringContent(JsonConvert.SerializeObject(userPayload), Encoding.UTF8, "application/json"));

            if (!createUserResponse.IsSuccessStatusCode)
            {
                var errorDetails = await createUserResponse.Content.ReadAsStringAsync();
                throw new Exception($"Failed to create user: {errorDetails}");
            }

            // Step 2: Extract the user ID from the Location header
            var locationHeader = createUserResponse.Headers.Location;
            if (locationHeader == null)
            {
                throw new Exception("Failed to retrieve user ID: Location header is missing.");
            }

            var userId = locationHeader.AbsolutePath.Split('/').Last();

            if (string.IsNullOrEmpty(userId))
            {
                throw new Exception("Failed to retrieve user ID from Location header.");
            }

            // Step 3: Set the user password
            var passwordPayload = new
            {
                type = "password",
                value = registerRequestDto1.Password,
                temporary = false
            };

            var setPasswordResponse = await _keycloakClient.Client.PutAsync(
                $"/admin/realms/{realm}/users/{userId}/reset-password",
                new StringContent(JsonConvert.SerializeObject(passwordPayload), Encoding.UTF8, "application/json"));

            if (!setPasswordResponse.IsSuccessStatusCode)
            {
                var errorDetails = await setPasswordResponse.Content.ReadAsStringAsync();
                throw new Exception($"Failed to set user password: {errorDetails}");
            }

            // Step 4: Trigger email verification
            var verifyEmailResponse = await _keycloakClient.Client.PutAsync(
                $"/admin/realms/{realm}/users/{userId}/send-verify-email",
                null); // Assuming no body required for this request

            if (!verifyEmailResponse.IsSuccessStatusCode)
            {
                var errorDetails = await verifyEmailResponse.Content.ReadAsStringAsync();
                throw new Exception($"Failed to send email verification: {errorDetails}");
            }
        }

        public Task ForgotPassword()
        {
            throw new NotImplementedException();
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            // Step 1: Obtain the admin token to interact with the Keycloak API
            var token = await _keycloakClient.GetAdminTokenAsync();
            var realm = "fluxpay";

            // Step 2: Validate if the user exists by username or email
            _keycloakClient.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var searchResponse = await _keycloakClient.Client.GetAsync(
              $"/admin/realms/{realm}/users?email={Uri.EscapeDataString(loginRequestDto.UserName)}");

            if (!searchResponse.IsSuccessStatusCode)
            {
                throw new Exception("Error while searching for the user.");
            }

            var users = await searchResponse.Content.ReadFromJsonAsync<List<KeycloakUserDto>>();

            if (users is null || !users.Any())
            {
                throw new Exception("User not found.");
            }

            var user = users.FirstOrDefault();

            if (user is null || user.emailVerified != true)
            {
                throw new Exception("User not found or email is not verified.");
            }

            var clientId = _configuration["Keycloak:ClientId"];
            var clientSecret = _configuration["Keycloak:ClientSecret"];

            var loginPayload = new Dictionary<string, string>
        {
            { "client_id", clientId },
            { "client_secret", clientSecret },
            { "username", loginRequestDto.UserName },
            { "password", loginRequestDto.Password },
            { "grant_type", "password" }
        };

            var response = await _keycloakClient.Client.PostAsync(
                $"/realms/{realm}/protocol/openid-connect/token",
                new FormUrlEncodedContent(loginPayload));

            if (!response.IsSuccessStatusCode)
            {
                var errorDetails = await response.Content.ReadAsStringAsync();
                throw new Exception("Invalid username or password");
            }

            var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponseDto>();

            return new LoginResponseDto
            {
                AccessToken = tokenResponse.access_token,
                RefreshToken = tokenResponse.refresh_token,
                ExpiresIn = tokenResponse.expires_in
            };
        }
    }

    public class TokenResponseDto
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public int expires_in { get; set; }
    }

}


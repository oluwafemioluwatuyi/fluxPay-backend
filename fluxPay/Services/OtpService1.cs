using AutoMapper;
using Dapper;
using fluxPay.Clients;
using fluxPay.DTOs;
using fluxPay.DTOs.AuthDtos;
using fluxPay.Interfaces.Repositories;
using fluxPay.Interfaces.Services;
using FluxPay.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;


namespace fluxPay.Services
{
    public class OtpService1 : IOtpService
    {
         private readonly FineractClient _fineractClient;
        public OtpService1(FineractClient fineractClient)
        {
             _fineractClient = fineractClient;
        }
        public async Task<bool> RequestOtpAsync(string email)
        {
             var url = $"/fineract-provider/api/v1/twofactor?deliveryMethod=sms&extendedToken=false";

        var requestBody = new
        {
            requestTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(), // Current timestamp in milliseconds
            tokenLiveTimeInSec = 300, // 5 minutes for the OTP validity
            extendedAccessToken = false,
            deliveryMethod = new
            {
                name = "email",
                target = email // Target phone number
            }
        };

        var jsonBody = JsonConvert.SerializeObject(requestBody);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        var response = await _fineractClient._client.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            // Optionally, you can handle response data if necessary (e.g., store a reference ID for later validation)
            return true;
        }

        // Handle failure - Log error or throw an exception
        return false;
        }

        public async Task<bool> ValidateOtpEmailAsync(string email, string otp)
        {
                var url =$"/fineract-provider/api/v1/twofactor/validate"; // Adjust endpoint for OTP validation

            var requestBody = new
            {
                target = email,  // The phone number where OTP was sent
                token = otp,           // The OTP entered by the user
            };

            var jsonBody = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await _fineractClient._client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                // OTP validated successfully
                return true;
            }

            // Handle failure (e.g., invalid OTP, expired OTP)
            return false;
        }
    }
}


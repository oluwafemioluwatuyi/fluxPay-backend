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
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;


namespace fluxPay.Services
{
    public class ClientService : IClientService
    {
        private readonly string _connectionString;
        public ClientService(string connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task<object> FindByEmailAsync(string mail)
        {
             using (var connection = new MySqlConnection(_connectionString)) // Use MySqlConnection
        {
            await connection.OpenAsync();
            
            var query = "SELECT * FROM m_client WHERE email_address = @Email_Address";
            return await connection.QueryFirstOrDefaultAsync<Client>(query, new { Email_Address = mail });
        }
        }

        public async Task<object> FindByPhoneNumberAsync(string phoneNumber)
        {
              using var connection = new MySqlConnection(_connectionString);
            // SQL query to check if the email exists in the m_client table
            const string query = "SELECT * FROM m_client WHERE mobile_no = @mobile_no";
            // Execute the query and fetch the result
             return await connection.QueryFirstOrDefaultAsync<Client>(query, new { mobile_no = phoneNumber });

        }

        public async Task<OtpConfigDto> GetOtpConfigureFromDb()
        {
             const string query = @"
        SELECT name, value 
        FROM twofactor_configuration 
        WHERE name LIKE 'otp-%'";

         var results = new Dictionary<string, string>();
         using (var connection = new MySqlConnection(_connectionString))
      {
        await connection.OpenAsync();

        using (var command = new MySqlCommand(query, connection))
        using (var reader = await command.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                results[reader["name"].ToString()] = reader["value"].ToString();
            }
        }
     }

     // Map the results to OtpConfigDto
        return new OtpConfigDto
        {
            OtpTokenLength = int.Parse(results.GetValueOrDefault("otp-token-length", "6")),
            OtpTokenExpiryTime = int.Parse(results.GetValueOrDefault("otp-token-live-time", "300")),
            OtpSubjectTemplate = results.GetValueOrDefault("otp-delivery-email-subject", "Your OTP Code"),
            OtpBodyTemplate = results.GetValueOrDefault("otp-delivery-email-body", "Your OTP code is {{token}}. It will expire in {{expiryTime}} minutes.")
        };

    }

        public async Task<SmtpSettings> GetSmtpSettingsFromDb()
        {
           const string query = @"
        SELECT 
            esp.name, esp.value
        FROM 
            c_external_service es
        JOIN 
            c_external_service_properties esp ON es.id = esp.external_service_id
        WHERE 
            es.name = 'SMTP_Email_Account'
            AND esp.name IN ('host', 'port', 'useTLS', 'username', 'password', 'fromEmail', 'fromName')";

    var results = new Dictionary<string, string>();

    using (var connection = new MySqlConnection(_connectionString))
    {
        await connection.OpenAsync();

        using (var command = new MySqlCommand(query, connection))
        using (var reader = await command.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                results[reader["name"].ToString()] = reader["value"].ToString();
            }
        }
    }

    // Map the results to SmtpSettings
    return new SmtpSettings
    {
    Host = results.ContainsKey("host") ? results["host"] : "smtp.example.com",
    Port = results.ContainsKey("port") ? int.Parse(results["port"]) : 25,
    UseTLS = results.ContainsKey("useTLS") ? bool.Parse(results["useTLS"]) : false,
    Username = results.ContainsKey("username") ? results["username"] : string.Empty,
    Password = results.ContainsKey("password") ? results["password"] : string.Empty,
    FromEmail = results.ContainsKey("fromEmail") ? results["fromEmail"] : "noreply@example.com",
    FromName = results.ContainsKey("fromName") ? results["fromName"] : "No Reply"
    };
        }
    };
}


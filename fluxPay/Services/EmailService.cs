using AutoMapper;
using fluxPay.Clients;
using fluxPay.DTOs;
using fluxPay.DTOs.AuthDtos;
using fluxPay.Interfaces.Repositories;
using fluxPay.Interfaces.Services;
using FluxPay.Models;
using FluxPay.Utils;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;


namespace fluxPay.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _templatesFolderPath;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IFineractApiService _fineractApiService;

        private readonly OtpService _otpService;

        private readonly IClientService _clientService;

        public EmailService(string templatesFolderPath, ILogger logger, IConfiguration configuration, IFineractApiService fineractApiService, IClientService clientService, OtpService otpService)
        {
            _templatesFolderPath = templatesFolderPath;
            _logger = logger;
            _configuration = configuration;
            _fineractApiService = fineractApiService;
            _clientService = clientService;
            _otpService = otpService;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string templateFileName, object model)
        {
            try
            {
                if (string.IsNullOrEmpty(toEmail))
                {
                    throw new ArgumentNullException(nameof(toEmail), "Recipient email address cannot be null or empty.");
                }

                var templateFileNameWithSuffix = templateFileName + ".html";
                var templatePath = Path.Combine(_templatesFolderPath, templateFileNameWithSuffix);

                _logger.LogInformation("Template path: {TemplatePath}", templatePath);

                if (!File.Exists(templatePath))
                {
                    throw new FileNotFoundException($"Template file not found: {templateFileNameWithSuffix}");
                }

                var templateContent = await File.ReadAllTextAsync(templatePath);
                //  var mergedContent = MergeTemplateWithModel(templateContent, model);

                var smtpSettings = await _fineractApiService.GetSmtpAsync();

                using (var smtpClient = new SmtpClient(smtpSettings.Host, smtpSettings.Port))
                {
                    smtpClient.EnableSsl = smtpSettings.UseTLS;
                    smtpClient.Credentials = new NetworkCredential(smtpSettings.Username, smtpSettings.Password);

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(smtpSettings.FromEmail, smtpSettings.FromName),
                        Subject = subject,
                        //  Body = mergedContent,
                        IsBodyHtml = true
                    };

                    _logger.LogInformation("Sending email to: {ToEmail}", toEmail);
                    mailMessage.To.Add(toEmail);

                    // Send the email asynchronously
                    await smtpClient.SendMailAsync(mailMessage);
                }

                _logger.LogInformation("Email sent successfully.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> SendOtpEmailAsync(string toEmail, string subject, string templateFileName, object model)
        {
            try
            {
                // Step 1: Generate OTP using Fineract (call OTP API)
                string token = _otpService.GenerateOtpCode(5);
                if (string.IsNullOrEmpty(token))
                {
                    throw new Exception("Failed to generate OTP.");
                }
                // Step 2: Load the HTML template from file
                var templateFilePath = Path.Combine("Emails", templateFileName + ".html");
                if (!File.Exists(templateFilePath))
                {
                    throw new FileNotFoundException($"Template file '{templateFileName}.html' not found.");
                }

                var bodyTemplate = await File.ReadAllTextAsync(templateFilePath);

                // Prepare the extended model
                var extendedModel = model.GetType()
                    .GetProperties()
                    .ToDictionary(prop => prop.Name, prop => prop.GetValue(model)?.ToString());



                // Step 3: Add the OTP token to the model
                extendedModel["token"] = token;
                var FirstName = model.GetType().GetProperty("FirstName")?.GetValue(model)?.ToString();
                if (!string.IsNullOrEmpty(FirstName))
                {
                    extendedModel["FirstName"] = FirstName; // Assign FirstName to UserName
                }

                Console.WriteLine("Extended Model: ");
                foreach (var kvp in extendedModel)
                {
                    Console.WriteLine($"{kvp.Key}: {kvp.Value}");
                }

                // Step 4: Merge the template with the extended model to populate placeholders
                var body = MergeTemplateWithModel(bodyTemplate, extendedModel);

                // Step 4: Retrieve SMTP settings from the database
                var smtpSettings = await _clientService.GetSmtpSettingsFromDb();

                // Step 5: Configure the SMTP client
                using (var smtpClient = new SmtpClient(smtpSettings.Host, smtpSettings.Port))
                {
                    smtpClient.EnableSsl = smtpSettings.UseTLS;
                    smtpClient.Credentials = new NetworkCredential(smtpSettings.Username, smtpSettings.Password);

                    // Step 6: Create the email message
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(smtpSettings.FromEmail, smtpSettings.FromName),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };
                    Console.WriteLine("Email body:");
                    Console.WriteLine(body);


                    mailMessage.To.Add(toEmail);

                    // Step 7: Send the email asynchronously
                    await smtpClient.SendMailAsync(mailMessage);
                }

                Console.WriteLine("Email sent successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while sending email: {ex.Message}");
                return false;
            }
        }

        // private string MergeTemplateWithModel(string templateContent, object model)
        // {
        //     foreach (var property in model.GetType().GetProperties())
        //     {
        //         var placeholder = $"{{{{{property.Name}}}}}";
        //         var value = property.GetValue(model)?.ToString();
        //         templateContent = templateContent.Replace(placeholder, value);
        //     }

        //     return templateContent;
        // }

        private string MergeTemplateWithModel(string template, Dictionary<string, string> model)
        {
            foreach (var keyValuePair in model)
            {
                var placeholder = $"{{{{{keyValuePair.Key}}}}}"; // Matches {{Key}}
                template = template.Replace(placeholder, keyValuePair.Value ?? string.Empty);
            }
            return template;
        }
    }

}
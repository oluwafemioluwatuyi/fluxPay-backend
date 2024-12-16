using AutoMapper;
using fluxPay.Clients;
using fluxPay.DTOs;
using fluxPay.DTOs.AuthDtos;
using fluxPay.Interfaces.Repositories;
using fluxPay.Interfaces.Services;
using FluxPay.Models;
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

    private readonly IClientService _clientService;

    public EmailService(string templatesFolderPath, ILogger logger, IConfiguration configuration, IFineractApiService fineractApiService, IClientService clientService)
    {
        _templatesFolderPath = templatesFolderPath;
        _logger = logger;
        _configuration = configuration;
        _fineractApiService = fineractApiService;
        _clientService = clientService;
    }

    public async Task<bool> SendEmailAsync(string toEmail, string subject, string templateFileName, object model)
    {
      try
        {
            // Step 1: Load the HTML template from file
            var templateFilePath = Path.Combine("Emails", templateFileName + ".html");
            if (!File.Exists(templateFilePath))
            {
                throw new FileNotFoundException($"Template file '{templateFileName}.html' not found.");
            }

            var templateContent = await File.ReadAllTextAsync(templateFilePath);

            // Step 2: Merge the template with the model data
            var mergedContent = MergeTemplateWithModel(templateContent, model);
            var smtpSettings = await _fineractApiService.GetSmtpAsync();

       using (var smtpClient = new SmtpClient(smtpSettings.Host, smtpSettings.Port))
           {
            smtpClient.EnableSsl = smtpSettings.UseTLS;
            smtpClient.Credentials = new NetworkCredential(smtpSettings.Username, smtpSettings.Password);

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpSettings.FromEmail, smtpSettings.FromName),
                Subject = subject,
                Body = mergedContent,
                IsBodyHtml = true
            };

            mailMessage.To.Add("test@mailtrap.io");

            // Send the email asynchronously
            await smtpClient.SendMailAsync(mailMessage);
        }


            Console.WriteLine("Email sent successfully.");

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
            throw;
        }
    }

        public async Task<bool> SendOtpEmailAsync(string toEmail, string subject, string body, object model)
        {
           try
    {
         var smtpSettings = new SmtpSettings
        {
            Host = "sandbox.smtp.mailtrap.io",
            Port = 2525,
            UseTLS = true,
            Username = "d400610248ca8f",
            Password = "5618657cc74a18",
            FromEmail = "your-email@example.com",
            FromName = "Your Name"
        };
        //var smtpSettings = await _clientService.GetSmtpSettingsFromDb();

        using (var smtpClient = new SmtpClient(smtpSettings.Host, smtpSettings.Port))
        {
            smtpClient.EnableSsl = smtpSettings.UseTLS;
            smtpClient.Credentials = new NetworkCredential(smtpSettings.Username, smtpSettings.Password);

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpSettings.FromEmail, smtpSettings.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            // Send the email asynchronously
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

        private string MergeTemplateWithModel(string templateContent, object model)
    {
        foreach (var property in model.GetType().GetProperties())
        {
            var placeholder = $"{{{{{property.Name}}}}}";
            var value = property.GetValue(model)?.ToString();
            templateContent = templateContent.Replace(placeholder, value);
        }

        return templateContent;
    }
}

}
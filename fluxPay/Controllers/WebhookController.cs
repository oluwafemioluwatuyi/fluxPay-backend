using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FluxPay.Models;
using fluxPay.Interfaces.Services;

namespace FluxPay.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebhookController : ControllerBase
    {
        private readonly ILogger<WebhookController> _logger;
        private readonly IEmailService _emailService;
        private readonly IWalletTransfer _walletTransfer;

        public WebhookController(ILogger<WebhookController> logger, IEmailService emailService, IWalletTransfer walletTransfer)
        {
            _logger = logger;
            _emailService = emailService;
            _walletTransfer = walletTransfer;
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> HandleWebhook([FromBody] WebhookEventPayload payload)
        {
            if (payload == null)
            {
                _logger.LogError("Received null payload");
                return BadRequest();
            }

            _logger.LogInformation("Received webhook event: {ActionName} for {EntityName}", payload.actionName, payload.entityName);

            // Process the payload
            await _walletTransfer.ProcessWebhookPayloadAsync(payload);

            return Ok();
        }


    }

    // public class WebhookEventPayload
    // {
    //     public string ActionName { get; set; }
    //     public string EntityName { get; set; }
    //     public dynamic Metadata { get; set; }
    // }
}
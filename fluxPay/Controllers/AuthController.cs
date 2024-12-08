using fluxPay.DTOs;
using fluxPay.DTOs.AuthDtos;
using fluxPay.Services;
using Microsoft.AspNetCore.Mvc;

namespace fluxPay.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }


    
          [HttpPost("register")]
         public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto, [FromQuery] AccountNumberFormatDto accountNumberFormat, int clientId, int productId, DateTime submittedOnDate)
        {
                if (registerRequestDto == null || accountNumberFormat == null)
                {
                    return BadRequest(new { Success = false, Message = "Invalid input data." });
                }
                    // Call the service to handle the registration process
                    var result = await _authService.Register(registerRequestDto, accountNumberFormat, clientId, productId, submittedOnDate);        
                    return StatusCode(500, new { Success = false, Message = "An unexpected error occurred. Please try again later." });            
        }

        // [HttpPost("register-client")]
        // public async Task<IActionResult> RegisterClient([FromBody] CreateClientRequestDto createClientRequestDto)
        // {
        //     try
        //     {
        //         // Register client using the AuthService
        //         var clientId = await _authService.RegisterClientAsync(createClientRequestDto);
        //         return Ok(new { message = "Client registered successfully", clientId });
        //     }
        //     catch (Exception ex)
        //     {
        //         return StatusCode(500, new { message = "An error occurred while registering the client", error = ex.Message });
        //     }
        // }

        // [HttpGet("get-client/{clientId}")]
        // public async Task<IActionResult> GetClient(int clientId, [FromQuery] bool staffInSelectedOfficeOnly)
        // {
        //     try
        //     {
        //         // Get client details using the AuthService
        //         var client = await _authService.GetClientAsync(clientId);
        //         return Ok(new { message = "Client retrieved successfully", client });
        //     }
        //     catch (Exception ex)
        //     {
        //         return StatusCode(500, new { message = "An error occurred while retrieving the client", error = ex.Message });
        //     }
        // }
    }
}

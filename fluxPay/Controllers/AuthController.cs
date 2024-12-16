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
         public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
                if (registerRequestDto == null)
                {
                    return BadRequest(new { Success = false, Message = "Invalid input data." });
                }
                    // Call the service to handle the registration process
                    var result = await _authService.Register(registerRequestDto);  
                    if(result is null)
                    {
                        return BadRequest(new { Success = false, Message = "Invalid input data." });

                    }
                    return StatusCode(201, new { Success = false, Message = "Successful." });            
        }

    }
}

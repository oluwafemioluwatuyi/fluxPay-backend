using fluxPay.DTOs;
using fluxPay.DTOs.AuthDtos;
using fluxPay.Interfaces.Services;
using fluxPay.Services;
using Microsoft.AspNetCore.Mvc;

namespace fluxPay.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly IKeyCloak _keyCloak;

        public AuthController(AuthService authService, IKeyCloak keyCloak)
        {
            _authService = authService;
            _keyCloak = keyCloak;
        }

        [HttpPost("registerUser")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterRequestDto1 registerRequestDto1)
        {
            if (registerRequestDto1 == null)
            {
                return BadRequest(new { Success = false, Message = "Invalid input data." });
            }
            // Call the service to handle the registration process
            await _keyCloak.CreateUser(registerRequestDto1);
            // if (result is null)
            // {
            //     return BadRequest(new { Success = false, Message = "Invalid input data." });

            // }
            return StatusCode(201, new { Success = false, Message = "Successful." });
        }

        // [HttpPost("register")]
        // public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        // {
        //     if (registerRequestDto == null)
        //     {
        //         return BadRequest(new { Success = false, Message = "Invalid input data." });
        //     }
        //     // Call the service to handle the registration process
        //     var result = await _authService.Register(registerRequestDto);
        //     if (result is null)
        //     {
        //         return BadRequest(new { Success = false, Message = "Invalid input data." });

        //     }
        //     return StatusCode(201, new { Success = false, Message = "Successful." });
        // }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var result = await _keyCloak.Login(loginRequestDto);
            if (result is null)
            {
                return BadRequest(new { Success = false, Message = "Invalid input data." });
            }
            return StatusCode(201, new { Success = true, Message = "Successful.", Data = result });

        }

        [HttpPost("Finialize-registration")]
        public async Task<IActionResult> CompleteRegistration([FromBody] RegisterRequestDto registerRequestDto)
        {
            var result = await _authService.FinializeRegister(registerRequestDto);
            if (result is null)
            {
                return BadRequest(new { Success = false, Message = "Invalid input data." });
            }
            return StatusCode(201, new { Success = true, Message = "Successful.", Data = result });

        }


    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using fluxPay.DTOs;
using fluxPay.DTOs.AuthDtos;
using fluxPay.Helpers;
using fluxPay.Services;


namespace fluxPay.Interfaces.Services;

public interface IKeyCloak
{
    Task CreateUser(RegisterRequestDto1 registerRequestDto1);
    Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);

    Task ForgotPassword();
}


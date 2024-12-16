using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using fluxPay.DTOs;
using fluxPay.DTOs.AuthDtos;
using fluxPay.Helpers;
using fluxPay.Services;


namespace fluxPay.Interfaces.Services;

public interface IFineractApiService
{
    Task<FineractApiResponse> CreateClientAsync(CreateClientRequestDto createClientRequestDto);
    Task<FineractApiResponse> CreateSavingAccount (CreateSavingsAccountRequestDto createSavingsAccountRequestDto );
    Task<FineractApiResponse> GetClientAsync(int clientId);

    Task<SmtpSettings> GetSmtpAsync();

    Task<OtpConfigDto> GetOtpConfigure();
    Task<AccountNumberFormatDto[]> ConfigureAccountNumber(AccountNumberFormatDto accountNumberFormat);

    Task<FineractApiResponse> CreateAccountNumber(AccountNumberFormatDto accountNumber, AccountTypeDto accountType, int clientId, int productId, DateTime submittedOnDate);

   
}

public class FineractCreateClientResponseBody
{

}

public class FineractGetClientResponseBody 
{

}
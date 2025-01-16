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
    Task<ClientApiResponse> CreateClientAsync(CreateClientRequestDto createClientRequestDto);
    Task<SavingsAccountResponseDto> CreateSavingAccount(CreateSavingsAccountRequestDto createSavingsAccountRequestDto);
    Task<FineractApiResponse> GetClientAsync(int clientId);

    Task<SmtpSettings> GetSmtpAsync();

    Task<FineractApiResponse> WebhookforTransfer();

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

public class CreateSavingsAccountResponseDto
{
    public long ResourceId { get; set; }
}

// public class SavingsAccountResponseDto
// {
//     public long ResourceId { get; set; }
//     public long ClientId { get; set; }
//     public long SavingsId { get; set; }
//     public long OfficeId { get; set; }
//     public string AccountNumber { get; set; }
//     public decimal Balance { get; set; }
//     public string AccountType { get; set; }
// }
public class SavingsAccountResponseDto
{
    public string AccountNumber { get; set; }
    public decimal Balance { get; set; }
    public string AccountType { get; set; }
}
public class SavingsAccountDetailsDto
{
    public string accountNo { get; set; }
    public depositType DepositType { get; set; }
    public summary Summary { get; set; }
}

public class depositType
{
    public string Value { get; set; }
}

public class summary
{
    public decimal AccountBalance { get; set; }
}


public class ClientApiResponse
{
    public int officeId { get; set; }
    public int clientId { get; set; }  // This might be missing or named differently
    public int resourceId { get; set; }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using fluxPay.DTOs;


namespace fluxPay.Interfaces.Services;

public interface IFineractApiService
{
    Task<string> CreateClientAsync(CreateClientRequestDto createClientRequestDto);
    Task<string> GetClientAsync(int clientId);
   
}

public class FineractCreateClientResponseBody
{

}

public class FineractGetClientResponseBody 
{

}
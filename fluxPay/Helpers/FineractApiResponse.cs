using System;


namespace fluxPay.Helpers;
public class FineractApiResponse
{
   

    public bool? Success { get; set; }
    public string? Message { get; set; }
    public object? Data { get; set; }
}

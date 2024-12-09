using System;
using System.Text;

namespace fluxPay.Utils;

public class AccountGenerator
{
    private static Random _random = new Random();

    // Generate 10-digit account number
    public static string GenerateAccountNumber()
    {
        // Generate a random 10-digit number
     string accountNumber = "1" + _random.Next(100000000, 1000000000).ToString();
        return accountNumber;
    }
}
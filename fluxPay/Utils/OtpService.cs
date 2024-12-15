namespace FluxPay.Utils;
public class OtpService
{
    private readonly Random _random;

    public OtpService()
    {
        _random = new Random();
    }

    // Method to generate OTP of a specific length
    public string GenerateOtpCode(int length)
    {
        // OTP consists of digits only (you can modify to support letters or special characters)
        const string otpChars = "0123456789";
        var otpCode = new char[length];
        for (int i = 0; i < length; i++)
        {
            otpCode[i] = otpChars[_random.Next(otpChars.Length)];
        }
        return new string(otpCode);
    }
}

using fluxPay.Interfaces;

namespace fluxPay.Constants
{
    public class Constants : IConstants
    {
        public int OTP_EXPIRY_TIME => throw new NotImplementedException();

        public int OTP_LENGTH => throw new NotImplementedException();

        public int EMAIL_VERIFICATION_TOKEN_EXPIRATION_TIME => 60;

        public int EMAIL_VERIFICATION_TOKEN_LENGTH => 6;
    }
}
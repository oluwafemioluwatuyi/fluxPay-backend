namespace fluxPay.Interfaces
{
    public interface IConstants
    {
        public int OTP_EXPIRY_TIME { get; }
        public int OTP_LENGTH { get; }
        public int EMAIL_VERIFICATION_TOKEN_EXPIRATION_TIME { get; }
        public int EMAIL_VERIFICATION_TOKEN_LENGTH { get; }
    }
}
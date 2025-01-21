using System;

namespace fluxPay.Constants;

public enum AppStatusCodes
{
    Success = 0000,
    BvnNotVerified = 0001,
    EmailNotVerified = 0002,
    PhoneAlreadyExists = 0003,
    InvalidCredentials = 0004,
    InvalidVerificationToken = 0005,
    NoWalletsLinked = 0006,
    NoPinCreated = 0007,
    AlreadyExists = 0008,
    ValidationError = 0009,
    ResourceNotFound = 0010,
    InvalidProvider = 0011,
    Unauthorized = 0012,
    PinAlreadyCreated = 00013,
    InsufficientFunds = 0014,
    InvalidOperation = 0015,
    InvalidData = 0016,
    InvalidAccountNumber = 0017,
    InternalServerError = 9999,
}

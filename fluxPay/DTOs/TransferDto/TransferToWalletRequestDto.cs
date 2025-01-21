public class TransferToWalletRequestDto
{
    public long AccountId { get; set; } // Source account ID
    public long DestinationAccountId { get; set; } // Destination account ID
    public string Locale { get; set; } = "en"; // Default to English
    public string DateFormat { get; set; } = "dd MMMM yyyy"; // Default date format
    public string TransactionDate { get; set; } // Date of the transaction
    public decimal TransactionAmount { get; set; } // Amount to deposit
    public int PaymentTypeId { get; set; } // Type of payment (e.g., bank transfer, cash)
    //public string AccountNumber { get; set; } // Account number for the transaction

}

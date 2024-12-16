using System.ComponentModel.DataAnnotations;

namespace fluxPay.DTOs.AuthDtos
{
    public class CreateSavingsAccountRequestDto
    { 
        public int clientId { get; set; } 
        public int productId {get; set;}
        public string Locale { get; set; } = "en"; // Default value
        public string DateFormat { get; set; } = "dd MMMM yyyy"; // Default value
         public string SubmittedOnDate { get; set; } 

    }
}

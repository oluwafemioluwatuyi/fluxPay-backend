using System.ComponentModel.DataAnnotations;

namespace fluxPay.DTOs.AuthDtos
{
   public class ClientDto
    {
        public int Id {get; set;}
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        // Other properties...
    }
}

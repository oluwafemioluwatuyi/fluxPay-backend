using System.ComponentModel.DataAnnotations;

namespace fluxPay.DTOs.AuthDtos
{
    public class KeycloakUserDto
    {
        public string username { get; set; }
        public string email { get; set; }
        public bool? emailVerified { get; set; }

    }
}

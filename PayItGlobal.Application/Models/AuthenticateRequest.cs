using System.ComponentModel.DataAnnotations;

namespace PayItGlobal.Application.Models
{
    public class AuthenticateRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public string UserIpAddress { get; set; }
    }
}

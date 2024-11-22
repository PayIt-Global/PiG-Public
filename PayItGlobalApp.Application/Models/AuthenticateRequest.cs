using System.ComponentModel.DataAnnotations;

namespace CryptAplyApp.Application.Models
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

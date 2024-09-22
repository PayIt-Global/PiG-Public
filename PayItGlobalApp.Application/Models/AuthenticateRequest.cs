using System.ComponentModel.DataAnnotations;

namespace PayItGlobalApp.Application.Models
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

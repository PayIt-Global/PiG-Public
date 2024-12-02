using Microsoft.AspNetCore.Identity;
using CryptAplyApi.Domain.Models;

namespace CryptAplyApi.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<int>, IApplicationUser
    {
        //example of adding a new property to the ApplicationUser
        public string FullName { get; set; }
    }
}
        
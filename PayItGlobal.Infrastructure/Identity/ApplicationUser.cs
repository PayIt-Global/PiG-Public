using Microsoft.AspNetCore.Identity;
using PayItGlobal.Domain.Models;

namespace PayItGlobal.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser, IApplicationUser
    {
        public string FullName { get; set; }
    }
}
        
using Microsoft.AspNetCore.Identity;
using PayItGlobal.Domain.Models;

namespace PayItGlobal.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<int>, IApplicationUser
    {
        //example of adding a new property to the ApplicationUser
        public string FullName { get; set; }
    }
}
        
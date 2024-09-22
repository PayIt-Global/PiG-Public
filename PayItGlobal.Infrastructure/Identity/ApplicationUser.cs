using Microsoft.AspNetCore.Identity;
using PayItGlobalApi.Domain.Models;

namespace PayItGlobalApi.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<int>, IApplicationUser
    {
        //example of adding a new property to the ApplicationUser
        public string FullName { get; set; }
    }
}
        
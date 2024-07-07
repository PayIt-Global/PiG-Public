using PayItGlobal.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PayItGlobalApi.Helper
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Example: Check if the user is authenticated
            var user = context.HttpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                // Not authenticated, return unauthorized
                context.Result = new UnauthorizedResult();
                return;
            }

            // Additional checks (e.g., roles, claims) can be performed here

            // If checks pass, do nothing to continue with the request
            // If checks fail, set context.Result as needed
        }
    }
}

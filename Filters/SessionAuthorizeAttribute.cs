using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Radiocab.Filters
{
    public class SessionAuthorizeAttribute : ActionFilterAttribute
    {
        public string Roles { get; set; } = string.Empty;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var email = session.GetString("UserEmail");
            var role = session.GetString("UserRole");

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(role))
            {
                context.Result = new RedirectToActionResult("Login", "MyController1", null);
                return;
            }

            if (!string.IsNullOrEmpty(Roles))
            {
                var allowedRoles = Roles.Split(',').Select(r => r.Trim().ToLowerInvariant());
                if (!allowedRoles.Contains(role.ToLowerInvariant()))
                {
                    context.Result = new RedirectToActionResult("Login", "MyController1", null);
                    return;
                }
            }

            base.OnActionExecuting(context);
        }
    }
}

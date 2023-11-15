using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;
using FifthGroup_front.Attributes;
namespace FifthGroup_front.Filter
{
    public class AuthenticationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var controllerName = context.RouteData.Values["controller"].ToString();
            var actionName = context.RouteData.Values["action"].ToString();
            if (controllerName == "Admin" && actionName == "Login" || (controllerName == "Admin" && actionName == "Register") || (controllerName == "Admin" && actionName == "ForgetPwd"))
            {
                return;
            }      
            if (context.ActionDescriptor.EndpointMetadata.OfType<WithoutAuthenticationAttribute>().Any())
            {
                return;
            }

            var userEmail = context.HttpContext.Session.GetString("UserEmail");
            if (userEmail == null)
            {
                context.Result = new RedirectToActionResult("Login", "Admin", null);
                Console.WriteLine("UserEmail from Session is null");
            }
            else
            {
                Console.WriteLine($"UserEmail from Session: {userEmail}");
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
         
        }
    }
}

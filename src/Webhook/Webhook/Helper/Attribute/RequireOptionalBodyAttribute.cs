using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Webhook.Helper.Attribute
{
    public class RequireOptionalBodyAttribute : ActionFilterAttribute
    {
        public RequireOptionalBodyAttribute()
        {
            // Run before the ModelStateInvalidFilter
            Order = -2001;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var controllerActionDescriptor = (ControllerActionDescriptor)context.ActionDescriptor;
            var methodParameters = controllerActionDescriptor.MethodInfo.GetParameters();
            for (var index = 0; index < context.ActionDescriptor.Parameters.Count; index++)
            {
                var parameter = context.ActionDescriptor.Parameters[index];
                if (parameter.BindingInfo?.BindingSource != BindingSource.Body)
                {
                    continue;
                }

                if (methodParameters[index].HasDefaultValue)
                {
                    continue;
                }

                var boundValue = context.ActionArguments[parameter.Name];
                if (boundValue != null) continue;
                context.ModelState.AddModelError(parameter.Name, "A non-empty request body is required");
                break;
            }
        }
    }
}

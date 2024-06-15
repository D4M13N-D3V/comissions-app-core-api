using Microsoft.AspNetCore.Mvc.Filters;

namespace comissions.app.api.Filters;

public class LoggingFilter : IActionFilter
{

    public LoggingFilter()
    {
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        // Log model state errors
        if (!context.ModelState.IsValid)
        {
            Console.WriteLine("Model validation failed: {@Errors}", context.ModelState);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Do nothing on action executed
    }
}
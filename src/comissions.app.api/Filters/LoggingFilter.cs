using Microsoft.AspNetCore.Mvc.Filters;

namespace comissions.app.database.Filters;

public class LoggingFilter : IActionFilter
{
    private readonly ILogger _logger;

    public LoggingFilter(ILogger<LoggingFilter> logger)
    {
        _logger = logger;
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
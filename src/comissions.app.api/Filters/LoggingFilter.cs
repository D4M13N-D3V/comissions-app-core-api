using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace comissions.app.api.Filters;

public class LoggingFilter : IActionFilter
{
    private readonly ILogger<LoggingFilter> _logger;

    public LoggingFilter(ILogger<LoggingFilter> logger)
    {
        _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        // Log model state errors
        if (!context.ModelState.IsValid)
        {
            _logger.LogWarning("Model validation failed: {@ModelState}", context.ModelState);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Do nothing on action executed
    }
}

public class DiagnosticMiddleware
{
    private readonly RequestDelegate _next;

    public DiagnosticMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault();
        Console.WriteLine($"Authorization Header: {token}");

        await _next(context);
    }
}


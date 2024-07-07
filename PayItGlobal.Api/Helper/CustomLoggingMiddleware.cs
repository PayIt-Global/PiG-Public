namespace PayEzPaymentApi.Helper
{
    public class CustomLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (token != null)
            {
                // Log the token or perform preliminary checks
                Console.WriteLine($"Token: {token}");
            }

            await _next(context);
        }
    }

}
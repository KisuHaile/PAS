using System.Diagnostics;
using System.Text;

namespace PAS.API.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var request = context.Request;

        // Log request
        if (_logger.IsEnabled(LogLevel.Information))
        {
            var requestBody = await ReadRequestBody(request);
            _logger.LogInformation(
                "HTTP {Method} {Path} started. Request Body: {RequestBody}",
                request.Method,
                request.Path,
                requestBody);
        }

        // Copy response stream
        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);
            stopwatch.Stop();

            // Log response
            if (_logger.IsEnabled(LogLevel.Information))
            {
                var responseBodyContent = await ReadResponseBody(context.Response);
                _logger.LogInformation(
                    "HTTP {Method} {Path} completed with {StatusCode} in {ElapsedMilliseconds}ms. Response: {ResponseBody}",
                    request.Method,
                    request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds,
                    responseBodyContent);
            }
        }
        finally
        {
            await responseBody.CopyToAsync(originalBodyStream);
        }
    }

    private async Task<string> ReadRequestBody(HttpRequest request)
    {
        request.EnableBuffering();

        var buffer = new byte[Convert.ToInt32(request.ContentLength)];
        await request.Body.ReadAsync(buffer, 0, buffer.Length);
        var bodyAsText = Encoding.UTF8.GetString(buffer);
        request.Body.Position = 0;

        return bodyAsText;
    }

    private async Task<string> ReadResponseBody(HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        var text = await new StreamReader(response.Body).ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);

        return text;
    }
}
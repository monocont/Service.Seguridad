using System.Text.Json;
using Service.Seguridad.Application.Common;

namespace Service.Seguridad.API.Middleware;

public class ResponseFormattingMiddleware
{
    private readonly RequestDelegate _next;

    public ResponseFormattingMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/swagger") ||
            context.Request.Path.StartsWithSegments("/.well-known"))
        {
            await _next(context);
            return;
        }

        var originalBodyStream = context.Response.Body;

        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);

            if (context.Response.ContentType != null && context.Response.ContentType.Contains("application/json"))
            {
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                using var jsonDoc = JsonDocument.Parse(responseText);
                var root = jsonDoc.RootElement;

                // Verificar si la respuesta ya es un ApiResponse para evitar doble envoltorio
                if (root.TryGetProperty("Success", out _) || root.TryGetProperty("success", out _))
                {
                    await responseBody.CopyToAsync(originalBodyStream);
                    return;
                }

                var apiResponse = ApiResponse<object>.SuccessResponse(
                    JsonSerializer.Deserialize<object>(responseText)!,
                    null
                );

                var responseBytes = JsonSerializer.SerializeToUtf8Bytes(apiResponse);
                context.Response.Body = originalBodyStream;
                await context.Response.Body.WriteAsync(responseBytes);
            }
            else
            {
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }
        catch (Exception)
        {
            // Los errores son manejados por ErrorHandlingMiddleware, 
            // pero nos aseguramos de que el stream se restaure y propagamos el error.
            context.Response.Body = originalBodyStream;
            throw;
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }
}

using System.Net;
using System.Text.Json;
using Service.Seguridad.Application.Common;
using Service.Seguridad.Application.Common.Exceptions;

namespace Service.Seguridad.API.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var apiResponse = new ApiResponse<object>
        {
            Success = false,
            Data = default,
            Messages = new List<string>()
        };

        switch (exception)
        {
            case UnauthorizedAccessException:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                apiResponse.Errors.Add(exception.Message);
                break;

            case KeyNotFoundException:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                apiResponse.Errors.Add(exception.Message);
                break;

            case ArgumentException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                apiResponse.Errors.Add(exception.Message);
                break;

            case ValidationException validationEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                foreach (var error in validationEx.Errors.SelectMany(e => e.Value))
                {
                    apiResponse.Errors.Add(error);
                }
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                apiResponse.Errors.Add("Error en el proceso");
                break;
        }

        var result = JsonSerializer.Serialize(apiResponse);
        return context.Response.WriteAsync(result);
    }
}

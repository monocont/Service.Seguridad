namespace Service.Seguridad.Application.Common;

public class ApiResponse<T>
{
    public T? Data { get; set; }
    public bool Success { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Messages { get; set; } = new();

    public static ApiResponse<T> SuccessResponse(T data, List<string>? messages = null)
    {
        return new ApiResponse<T>
        {
            Data = data,
            Success = true,
            Messages = messages ?? new List<string>()
        };
    }

    public static ApiResponse<T> ErrorResponse(string error, List<string>? messages = null)
    {
        return new ApiResponse<T>
        {
            Data = default,
            Success = false,
            Errors = new List<string> { error },
            Messages = messages ?? new List<string>()
        };
    }
}

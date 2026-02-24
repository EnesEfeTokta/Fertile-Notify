namespace FertileNotify.API.Models.Responses
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        public static ApiResponse<T> SuccessResult(T data, string message = default!)
            => new ApiResponse<T> { Success = true, Data = data, Message = message };

        public static ApiResponse<T> FailureResult(List<string> errors, string message = default!)
            => new ApiResponse<T> { Success = false, Errors = errors, Message = message };
    }
}

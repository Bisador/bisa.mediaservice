namespace MediaService.Api.Middlewares;

public class AppException : Exception
{
    public int StatusCode { get; set; }
}
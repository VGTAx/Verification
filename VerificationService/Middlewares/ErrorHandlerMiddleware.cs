namespace VerificationApp.Middlewares
{
  /// <summary>
  /// Error handling middleware
  /// </summary>
  public class ErrorHandlerMiddleware : IMiddleware
  {
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
      try
      {
        await next(context);
      }
      catch
      {      
        context.Response.Clear();
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
      }
    }
  }
}

using Microsoft.AspNetCore.Http;

namespace Ecommerce.SharedLibrary.Middleware
{
    public class ListenToOnlyAPIGateway(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            //extract specific header from the request
            var signedHeader = context.Request.Headers["Api-Gateway"];

            //Null means the request is not coming from the API Gateway //503 service unavaliable
            if (signedHeader.FirstOrDefault() == null)
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync("Sorry, service is unavailable");
                return;
            }
            else
            {
                await next(context);
            }

        }
    }
}

using Ecommerce.SharedLibrary.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace Ecommerce.SharedLibrary.Middleware
{
    public class GlobalException(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            //Declare default variables
            string message = "Sorry, internal server error occurrend. Kindly try again!!";
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string title = "Error";


            try
            {
                await next(context);

                //check if exception is too many request //429 status code.
                if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
                {
                    title = "Warning";
                    message = "Too many request";
                    statusCode = (int)StatusCodes.Status429TooManyRequests;
                    await ModifyHeader(context, title, message, statusCode);

                }
                //check if response is unauthorize //401 status code
                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    title = "Alert";
                    message = "Your are not authorized to access";
                    statusCode = (int)StatusCodes.Status401Unauthorized;

                    await ModifyHeader(context, title, message, statusCode);
                }

                //if response is forbidden //403

                if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    title = "Out of access";
                    message = "Your are not allow/require to access";
                    statusCode = (int)StatusCodes.Status403Forbidden;

                    await ModifyHeader(context, title, message, statusCode);
                }

            }
            catch (Exception ex)
            {
                //Log original exceptions/file, debugger, console
                LogException.LogExceptions(ex);

                //check if exception is timeout
                if (ex is TaskCanceledException || ex is TimeoutException)
                {
                    title = "Out of time";
                    message = "REquest timeout .... try again";
                    statusCode = StatusCodes.Status408RequestTimeout;
                }


                // if none of the exceptions then do the default(at declare)
                // if exceptions is caught 

                await ModifyHeader(context, title, message, statusCode);
            
            }
        }

        private async Task ModifyHeader(HttpContext context, string title, string message, int statusCode)
        {
            //display scary-frr message to client
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails()
            {
                Detail = message,
                Status = statusCode,
                Title = title,
            }), CancellationToken.None);
            return;
        }
    }
}

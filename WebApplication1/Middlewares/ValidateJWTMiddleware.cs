using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Repos;

namespace WebApplication1.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ValidateJWTMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ValidateJWTMiddleware> _logger;
        public ValidateJWTMiddleware(RequestDelegate next, ILogger<ValidateJWTMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext, IEncryptRepo _eRepo)
        {
            if (httpContext.Request.Path.StartsWithSegments("/api/auth"))
                await _next(httpContext);

            else
            {
                string userId = string.Empty;
                Task task = Task.Run(async () =>
                {
                    // now validate the token
                    StringValues token;
                    if (httpContext.Request.Headers.ContainsKey("authToken"))
                    {
                        _logger.LogInformation("token key exist in header...");
                        if (httpContext.Request.Headers.TryGetValue("authToken", out token))
                        {
                            _logger.LogInformation("token acquired...");
                            userId = await _eRepo.ValidateToken(token);
                            if (userId != null)
                                _logger.LogInformation("user id: " + userId);
                            else
                                await _next.EndInvoke(null);
                        }
                        else
                        {
                            _logger.LogWarning("no auth token's value in request header...");
                            await _next.EndInvoke(null);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("no auth token's key in request header...");
                        await _next.EndInvoke(null);
                    }
                });

                task.Wait();
                if (task.IsCompleted)
                {
                    if (!string.IsNullOrEmpty(userId))
                        await _next(httpContext);
                }
            }

        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ValidateJWTMiddlewareExtensions
    {
        public static IApplicationBuilder UseValidateJWT(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ValidateJWTMiddleware>();
        }
    }
}

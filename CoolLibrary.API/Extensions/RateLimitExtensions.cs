using AspNetCoreRateLimit;
using AspNetCoreRateLimit.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System.Security.Claims;

namespace CoolLibrary.API.Extensions
{
    public static class RateLimitExtensions
    {
        public static IServiceCollection AddRedisRateLimiting(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Configure memory cache for rate limiting
            services.AddMemoryCache();

            // Configure Client-based rate limiting (supports both IP and authenticated users)
            services.Configure<ClientRateLimitOptions>(options =>
            {
                // General rule: 20 requests per minute
                options.GeneralRules = new List<RateLimitRule>
                {
                    new RateLimitRule
                    {
                        Endpoint = "*",           // Applies to all endpoints
                        Period = "1m",            // 1 minute period
                        Limit = 20                // Maximum 20 requests
                    }
                };

                // Additional configuration
                options.EnableEndpointRateLimiting = true;
                options.StackBlockedRequests = false;  // Don't stack blocked requests
                options.HttpStatusCode = 429;          // HTTP Code: Too Many Requests
                options.RealIpHeader = "X-Real-IP";    // Header to get real IP (useful with proxies)
                options.ClientIdHeader = "X-ClientId"; // Alternative header to identify client
                
                // Custom error message
                options.QuotaExceededResponse = new QuotaExceededResponse
                {
                    Content = "{{ \"message\": \"Rate limit exceeded. Maximum 20 requests per minute.\", \"retryAfter\": \"{1}\" }}",
                    ContentType = "application/json",
                    StatusCode = 429
                };
            });

            // Configure rate limiting policies
            services.Configure<ClientRateLimitPolicies>(options =>
            {
                options.ClientRules = new List<ClientRateLimitPolicy>();
            });

            // Get Redis connection string
            var redisConnection = configuration.GetConnectionString("Redis");

            if (!string.IsNullOrEmpty(redisConnection) && redisConnection != "disabled")
            {
                // Use Redis to store rate limiting counters (distributed)
                var connectionMultiplexer = ConnectionMultiplexer.Connect(redisConnection);
                services.AddSingleton<IConnectionMultiplexer>(connectionMultiplexer);
                
                // Use Redis for storing configuration and counters
                services.AddRedisRateLimiting();
            }
            else
            {
                // Fallback to in-memory storage (development only)
                services.AddInMemoryRateLimiting();
            }

            // Register required services
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            // Custom client resolver: Use UserID from JWT if authenticated, otherwise use IP
            services.AddSingleton<IClientResolveContributor, JwtClientResolveContributor>();

            return services;
        }
    }

    /// <summary>
    /// Custom resolver to identify clients by JWT UserID (authenticated) or IP (anonymous)
    /// </summary>
    public class JwtClientResolveContributor : IClientResolveContributor
    {
        public Task<string> ResolveClientAsync(Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            string clientId;

            // Check if user is authenticated and has a valid JWT token
            if (httpContext.User?.Identity?.IsAuthenticated == true)
            {
                // Try to get UserID from JWT claims (NameIdentifier is the standard claim for UserID)
                var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? httpContext.User.FindFirst("sub")?.Value  // Alternative claim name
                          ?? httpContext.User.FindFirst("userId")?.Value; // Custom claim fallback

                if (!string.IsNullOrEmpty(userId))
                {
                    // Use UserID as client identifier for authenticated users
                    clientId = $"user_{userId}";
                }
                else
                {
                    // Fallback to IP if no UserID found in token
                    clientId = $"ip_{GetClientIp(httpContext)}";
                }
            }
            else
            {
                // Anonymous users: use IP address
                clientId = $"ip_{GetClientIp(httpContext)}";
            }

            return Task.FromResult(clientId);
        }

        private string GetClientIp(Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            // Try to get real IP from headers (useful behind proxies/load balancers)
            var ip = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault()
                  ?? httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                  ?? httpContext.Connection.RemoteIpAddress?.ToString()
                  ?? "unknown";

            return ip;
        }
    }
}

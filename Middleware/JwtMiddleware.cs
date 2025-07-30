using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ProjectManager.Api.Services;

namespace ProjectManager.Api.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        Console.WriteLine($"JwtMiddleware: Processing request to {context.Request.Path}");
        Console.WriteLine($"JwtMiddleware: Authorization header present: {!string.IsNullOrEmpty(token)}");

        if (token != null)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found"));
                
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

                Console.WriteLine($"JwtMiddleware: Token validated successfully, User ID: {userId}");

                // Add user ID to context items for use in controllers
                context.Items["UserId"] = userId;
                
                // Also set the user principal for the standard authentication system
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim(ClaimTypes.Email, jwtToken.Claims.First(x => x.Type == ClaimTypes.Email).Value),
                    new Claim(ClaimTypes.Name, jwtToken.Claims.First(x => x.Type == ClaimTypes.Name).Value)
                };
                
                var identity = new ClaimsIdentity(claims, "Bearer");
                context.User = new ClaimsPrincipal(identity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"JwtMiddleware: Token validation failed: {ex.Message}");
                // Token validation failed, but we don't throw here
                // The [Authorize] attribute will handle unauthorized requests
            }
        }
        else
        {
            Console.WriteLine("JwtMiddleware: No authorization token found");
        }

        await _next(context);
    }
} 
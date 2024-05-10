using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using UserService.Infrastructure;
using UserService.Infrastructure.Options;

namespace UserService.WebApi.Extensions
{
    public static class ApiExtensions
    {
        public static void AddApiAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtOptions = new JwtOptions();
            configuration.GetSection(nameof(JwtOptions)).Bind(jwtOptions);
            var cookies = new MyCookiesOptions();
            configuration.GetSection(nameof(MyCookiesOptions)).Bind(cookies);
            var gitOptions = new GithubOptions();
            configuration.GetSection(nameof(GithubOptions)).Bind(gitOptions);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddCookie("cookie")
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => 
                {
                    options.TokenValidationParameters = new() 
                    {
                        ValidateIssuer = jwtOptions.ValidateIssuer,
                        ValidateAudience = jwtOptions.ValidateAudience,
                        ValidateLifetime = jwtOptions.ValidateLifetime,
                        ValidateIssuerSigningKey = jwtOptions.ValidateIssuerSigningKey,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                        //ClockSkew = new TimeSpan(0,0,5) uncomment for testing purposes
                    };
                    // get token from cookies
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            context.Token = context.Request.Cookies[cookies.TokenFieldName];

                            return Task.CompletedTask;
                        }
                    };
                })
                .AddGitHub("github",options =>
                {
                    options.SignInScheme = "cookie";
                    options.ClientId = gitOptions.ClientId;
                    options.ClientSecret = gitOptions.SecretKey;
                    options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
                    options.TokenEndpoint = "https://github.com/login/oauth/access_token";
                    options.UserInformationEndpoint = "https://api.github.com/user";
                    options.CallbackPath = "/api/auth/signin-github";
                });
            services.AddAuthorization();
        }
    }
}
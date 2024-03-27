using System.Reflection;
using EventBus.RabbitMQ.Standard.Configuration;
using EventBus.RabbitMQ.Standard.Options;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Services;
using UserService.Core.Interfaces;
using UserService.Core.Interfaces.Auth;
using UserService.Core.Interfaces.Infrastructure;
using UserService.Core.Interfaces.Repositories;
using UserService.Core.Interfaces.Services;
using UserService.DataAccess;
using UserService.DataAccess.Repositories;
using UserService.Infrastructure;
using UserService.Infrastructure.Options;
using UserService.WebApi.Extensions;
using UserService.WebApi.Handlers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddDbContext<UserServiceContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(nameof(JwtOptions)));
builder.Services.Configure<MyCookiesOptions>(builder.Configuration.GetSection(nameof(MyCookiesOptions)));
builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection(nameof(EmailOptions)));

builder.Services.AddApiAuthentication(builder.Configuration);

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBadgeRepository, BadgeRepository>();

builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IBadgeService, BadgeService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// var rabbitMqOptins = builder.Configuration.GetSection("RabbitMq").Get<RabbitMqOptions>();

// builder.Services.AddRabbitMqConnection(rabbitMqOptins);
// builder.Services.AddRabbitMqRegistration(rabbitMqOptins);
//builder.Services.AddMassTransitWithRabbitMQ();

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("InDocker"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseRouting();
app.UseHttpsRedirection();

app.UseCookiePolicy(new CookiePolicyOptions
{
    Secure = CookieSecurePolicy.Always
});

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(ep => ep.MapControllers());

app.Run();
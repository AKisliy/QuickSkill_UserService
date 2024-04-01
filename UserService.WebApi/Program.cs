using System.Reflection;
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

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddDbContext<UserServiceContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(nameof(JwtOptions)));
builder.Services.Configure<MyCookiesOptions>(builder.Configuration.GetSection(nameof(MyCookiesOptions)));
builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection(nameof(EmailOptions)));
builder.Services.Configure<YandexDiskOptions>(builder.Configuration.GetSection(nameof(YandexDiskOptions)));
builder.Services.Configure<RabbitMQOptions>(builder.Configuration.GetSection(nameof(RabbitMQOptions)));

builder.Services.AddApiAuthentication(builder.Configuration);

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBadgeRepository, BadgeRepository>();

builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IBadgeService, BadgeService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<IImageProvider, ImageProvider>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// var rabbitMqOptins = builder.Configuration.GetSection("RabbitMq").Get<RabbitMqOptions>();

// builder.Services.AddRabbitMqConnection(rabbitMqOptins);
// builder.Services.AddRabbitMqRegistration(rabbitMqOptins);
builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowSpecificOrigin",
            builder =>
            {
                builder.WithOrigins("http://localhost:3000") // Specify the origin of your client application
                       .AllowAnyHeader()
                       .AllowAnyMethod();
            });
    }
);

builder.Services.AddMassTransitWithRabbitMQ(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("InDocker"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowSpecificOrigin");
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
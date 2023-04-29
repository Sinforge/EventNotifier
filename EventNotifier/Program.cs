using EventNotifier.Data;
using EventNotifier.Repositories;
using Microsoft.EntityFrameworkCore;
using EventNotifier.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(option =>
    {
        var jwtConfig = builder.Configuration.GetSection("Audience");
        option.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtConfig["Secret"])),
            ValidateIssuer = true,
            ValidIssuer = jwtConfig["Iss"],
            ValidateAudience = true,
            ValidAudience = jwtConfig["Aud"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            RequireExpirationTime = true,

        };
    });
builder.Services.AddAuthorization();
builder.Services.Configure<EventNotifier.Controllers.Audience>(builder.Configuration.GetSection("Audience"));
builder.Services.Configure<EventNotifier.Services.EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString(name: "DefaultConnection")));
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEventRepo, EventRepo>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
// Configure the HTTP request pipeline.
app.MapControllerRoute(
    name: "default",
    pattern: "api/{controller=User}/{action=Registration}");




app.Run();


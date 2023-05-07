using EventNotifier.Data;
using EventNotifier.Repositories;
using Microsoft.EntityFrameworkCore;
using EventNotifier.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Hangfire;
using Hangfire.PostgreSql;
using EventNotifier.Infrastructure.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NetTopologySuite.Geometries;
using NetTopologySuite;
using NetTopologySuite.IO.Converters;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options => {
    options.JsonSerializerOptions.Converters.Add(new NetTopologySuite.IO.Converters.GeoJsonConverterFactory());
});
builder.Services.AddSingleton(NtsGeometryServices.Instance);

Console.WriteLine(builder.Configuration.GetConnectionString(name: "DefaultConnection"));
builder.Services.AddDbContext<ApplicationDbContext>(
    options =>
    {
        options.UseLazyLoadingProxies().UseNpgsql(
            builder.Configuration.GetConnectionString(name: "DefaultConnection"),
            serverOption => serverOption.UseNetTopologySuite());
    }
    );
builder.Services.AddHangfire(h => h
    .UsePostgreSqlStorage(builder.Configuration.GetConnectionString(name: "DefaultConnection")));
builder.Services.AddHangfireServer();

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

builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEventRepo, EventRepo>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IRecommendationService, RecommendationService>();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ApplicationDbContext>();
    if (context.Database.GetPendingMigrations().Any())
    {
        context.Database.Migrate();
    }
}   

app.UseHangfireDashboard(
options: new DashboardOptions
{
    Authorization = new[]
        {
        new HangfireAuthorizationFilter{ }
        }
}
) ;
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
// Configure the HTTP request pipeline.

app.MapControllerRoute(
    name: "default",
    pattern: "api/{controller=User}/{action=Registration}");




app.Run();


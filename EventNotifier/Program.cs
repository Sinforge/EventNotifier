using EventNotifier.Data;
using EventNotifier.Repositories;
using Microsoft.EntityFrameworkCore;
using EventNotifier.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString(name: "DefaultConnection")));
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();


// Configure the HTTP request pipeline.
app.MapControllerRoute(
    name: "default",
    pattern: "api/{controller=Home}/{action=Index}");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();


app.Run();


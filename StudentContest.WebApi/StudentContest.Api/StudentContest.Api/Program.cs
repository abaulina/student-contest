using Microsoft.EntityFrameworkCore;
using StudentContest.Api.Authorization;
using StudentContest.Api.ExceptionMiddleware;
using StudentContest.Api.Helpers;
using StudentContest.Api.Models;
using StudentContest.Api.Services;
using StudentContest.Api.Validation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<UserContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Sqlite")));

builder.Services.AddCors(options => options.AddPolicy("ApiCorsPolicy", corsPolicyBuilder =>
{
    corsPolicyBuilder.AllowAnyOrigin().AllowAnyMethod()
        .AllowAnyHeader();
}));
builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

builder.Services.AddSingleton<ILogger, FileLogger>();
builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddSingleton<IJwtUtils,JwtUtils>();
builder.Services.AddScoped<IRegisterRequestValidator, RegisterRequestValidator>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<UserContext>();
    if (!context.Database.IsInMemory())
        context.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseRouting(); //?

app.UseCors("ApiCorsPolicy");

app.UseAuthorization(); //?

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseMiddleware<JwtMiddleware>();

app.MapControllers();

app.Run();

public partial class Program { }

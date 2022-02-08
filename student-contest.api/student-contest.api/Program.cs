using Microsoft.EntityFrameworkCore;
using student_contest.api.Authorization;
using student_contest.api.ExceptionMiddleware;
using student_contest.api.Helpers;
using student_contest.api.Models;
using student_contest.api.Services;
using student_contest.api.Validation;

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

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StudentContest.Api.Auth;
using StudentContest.Api.ExceptionMiddleware;
using StudentContest.Api.Models;
using StudentContest.Api.Services;
using StudentContest.Api.Services.NoteRepository;
using StudentContest.Api.Services.RefreshTokenRepository;
using StudentContest.Api.Validation;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Sqlite")));

builder.Services.AddCors(options => options.AddPolicy("ApiCorsPolicy", corsPolicyBuilder =>
{
    corsPolicyBuilder.WithOrigins("http://localhost:3000", "https://localhost:3000", "https://localhost:7050").AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
}));
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Description = "An ASP.NET Core Web API",
    });
});
builder.Services.AddIdentityCore<User>(u =>
    {
        u.Password.RequiredLength = 8;
        u.Password.RequireDigit = false;
        u.Password.RequireUppercase = false;
        u.User.RequireUniqueEmail = true;
        u.Password.RequireNonAlphanumeric = false;
        u.Password.RequireLowercase = false;
    }
)
    .AddRoles<IdentityRole<int>>()
    .AddEntityFrameworkStores<ApplicationContext>();


var authenticationConfiguration = new AuthenticationConfiguration();
builder.Configuration.Bind("Authentication", authenticationConfiguration);
builder.Services.AddSingleton(authenticationConfiguration);

builder.Services.AddSingleton<ILogger, FileLogger>();
builder.Services.AddSingleton<ITokenGenerator, TokenGenerator>();
builder.Services.AddSingleton<RefreshTokenValidator>();
builder.Services.AddScoped<Authenticator>();
builder.Services.AddScoped<IRegisterRequestValidator, RegisterRequestValidator>();
builder.Services.AddScoped<INoteValidator, NoteValidator>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IAuthService,AuthService>();
builder.Services.AddScoped<INoteService, NoteService>();
builder.Services.AddScoped<IUserRolesService, UserRolesService>();
builder.Services.AddScoped<IRefreshTokenRepository, DatabaseRefreshTokenRepository>();
builder.Services.AddScoped<INoteRepository, DatabaseNoteRepository>();
builder.Services.AddScoped<IUserManagerWrapper, UserManagerWrapper>();
builder.Services.AddScoped<IRoleManagerWrapper, RoleManagerWrapper>();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationConfiguration.AccessTokenSecret)),
        ValidIssuer = authenticationConfiguration.Issuer,
        ValidAudience = authenticationConfiguration.Audience,
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ClockSkew = TimeSpan.Zero
    };
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
    if (!context.Database.IsInMemory())
        context.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseCors("ApiCorsPolicy");

app.UseAuthentication();

app.UseAuthorization(); 

app.UseMiddleware<ErrorHandlingMiddleware>();

app.MapControllers();

app.Run();

public partial class Program { }

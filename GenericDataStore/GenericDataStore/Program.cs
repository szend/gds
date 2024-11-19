using FluentScheduler;
using GenericDataStore;
using GenericDataStore.DatabaseConnector;
using GenericDataStore.Models;
using GenericDataStore.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddTransient<ApplicationDbContext>(option =>
//{
//    option.UseSqlServer("Name=ConnectionStrings:Db");
//});

builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseSqlServer("Name=ConnectionStrings:Db");

    //option.UseMySql("Server=localhost;Database=mysql;User ID=root;Password=;Port=3306;",
    //   ServerVersion.AutoDetect("Server=localhost;Database=mysql;User ID=root;Password=;Port=3306;"));


    //option.UseMySql("SSL Mode=Required;Server=mysql-3e45a36c-gdsdatabase.c.aivencloud.com;Database=defaultdb;User=avnadmin;Password=AVNS_VjKtKwWLTnm6d65ufau;port=11348;SslMode=Required;SslCa=DigiCertGlobalRootCA.crt.pem;Allow User Variables=true;",
    //   ServerVersion.AutoDetect("SSL Mode=Required;Server=mysql-3e45a36c-gdsdatabase.c.aivencloud.com;Database=defaultdb;User=avnadmin;Password=AVNS_VjKtKwWLTnm6d65ufau;port=11348;SslMode=Required;SslCa=DigiCertGlobalRootCA.crt.pem;Allow User Variables=true"));



});

builder.Services.AddMemoryCache();

builder.Services.AddCors(c =>
{
    c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod().
     AllowAnyHeader());
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
            )
        };
    });

builder.Services
    .AddIdentity<AppUser, AppRole>(options => {
        options.SignIn.RequireConfirmedAccount = false;
        options.User.RequireUniqueEmail = true;
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
    })

    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<JwtService>();



builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Full", policy =>
    {
        policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser();

    });
});


var app = builder.Build();
//JobManager.Initialize();
//JobManager.AddJob(() => {
//    new ScheduledTask().Start();
//}, s => s.ToRunEvery(5).Days());
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowOrigin");

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseDefaultFiles();
app.UseStaticFiles();


app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("index.html");

app.Run();

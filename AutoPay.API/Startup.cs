using System.Text.Json;
using AutoPay.API.Controllers;
using AutoPay.API.DB;
using AutoPay.API.Services;
using AutoPay.API.Services.Abstractions;
using AutoPay.API.Services.Configuration;
using AutoPay.Common.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AutoPay.API;

public class Startup
{
    public const string JwtSecretKey = "JWT:SecretKey";

    private readonly IAppConfiguration _configuration;

    public Startup(IAppConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var jwtPrivateKeyString =
            _configuration.GetOrSetDefault(JwtSecretKey, () => GenerateDefaultJwtKey(new JwtPrivateKeyGenerator()));
        var jwtPrivateKey = Convert.FromBase64String(jwtPrivateKeyString);
        var tokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKey = new SymmetricSecurityKey(jwtPrivateKey),
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
        };

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => { options.TokenValidationParameters = tokenValidationParameters; });
        services.AddSingleton<IPasswordHashService, ShaPasswordHashService>();

        const string connectionStringKey = "DataBase:ConnectionString";
        var connectionString = _configuration.GetOrSetDefault(connectionStringKey, () => "DataSource=data.db");

        services.AddDbContext<DataContext>(o => o.UseSqlite(connectionString));
        services.AddControllers(o => o.EnableEndpointRouting = false);
        services.AddSwaggerGen();

        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        services.AddScoped<IImageUrlBuilder, ImageUrlBuilder>(p => new ImageUrlBuilder(
            "/api/images",
            p.GetRequiredService<IActionContextAccessor>()));
    }

    private string GenerateDefaultJwtKey(IJwtPrivateKeyGenerator generator)
    {
        return Convert.ToBase64String(generator.Generate());
    }

    private string GenerateNewLocalAdminPassword()
    {
        var random = new Random();
        var passwordBuffer = new byte[6];
        random.NextBytes(passwordBuffer);
        return Convert.ToBase64String(passwordBuffer);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IPasswordHashService passwordHashService,
        DataContext dataContext)
    {
        dataContext.Database.Migrate();

        if (!dataContext.AccessRights.Any())
        {
            dataContext.AccessRights.Add(new AccessRight()
            {
                Id = Guid.NewGuid(),
                Type = AccessRightType.Admin
            });
            dataContext.AccessRights.Add(new AccessRight()
            {
                Id = Guid.NewGuid(),
                Type = AccessRightType.User
            });
            dataContext.SaveChanges();
        }

        const string configPasswordKey = "Administration:LocalAdminPassword";
        var localAdminPassword = _configuration.GetOrSetDefault(configPasswordKey, GenerateNewLocalAdminPassword);

        var user = dataContext.Users.FirstOrDefault(u => u.UserName == User.LocalAdminUserName);
        if (user is not null && !passwordHashService.ValidateHash(user.PasswordHash, localAdminPassword))
        {
            dataContext.Users.Remove(user);
            dataContext.SaveChanges();
            user = null;
        }

        if (user is null)
        {
            var accessRights = dataContext.AccessRights.ToList();

            var localAdmin = new User()
            {
                Id = Guid.NewGuid(),
                UserName = User.LocalAdminUserName,
                FullName = "Administrator",
                PasswordHash = passwordHashService.CalculateHash(localAdminPassword),
                AccessRights = accessRights
            };

            dataContext.Users.Add(localAdmin);
            dataContext.SaveChanges();
        }

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(opt => { opt.SwaggerEndpoint("/swagger/v1/swagger.json", "v1"); });
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMvc();
    }
}
using IdentityWebApi.Extensions;
using Microsoft.AspNetCore.Identity;
using IdentityWebApi.Persistense.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using IdentityWebApi.Settings;
using IdentityWebApi.Services;
using IdentityWebApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtSettings>(options =>
    builder.Configuration.GetSection(JwtSettings.SectionKey).Bind(options));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite("Data Source=C:\\Temp\\identity-web-api\\src\\IdentityWebApi\\db\\IdentityWebApi.sqlite");
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(
        authenticationScheme: JwtBearerDefaults.AuthenticationScheme,
        options =>
        {
            var jwtSettings = new JwtSettings();
            builder.Configuration.GetSection(JwtSettings.SectionKey)
                .Bind(jwtSettings);

            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.IncludeErrorDetails = true;
            options.Audience = jwtSettings.Audience;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ClockSkew = TimeSpan.Zero,

                ValidAudience = jwtSettings.Audience,
                ValidateAudience = true,
                
                ValidIssuer = jwtSettings.Issuer,
                ValidateIssuer = true,
                
                IssuerSigningKey = JwtSettings.SecurityKey,
                ValidateIssuerSigningKey = true,
                
                LifetimeValidator = CustomLifetimeValidator,
                ValidateLifetime = true
            };

            static bool CustomLifetimeValidator(
                DateTime? notBefore, 
                DateTime? expires,
                Microsoft.IdentityModel.Tokens.SecurityToken securityToken, 
                TokenValidationParameters validationParameters) 
                    => expires is not null && expires > DateTime.UtcNow;
    });

builder.Services.AddCors();
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();

builder.Services.AddTransient<ITokenService, TokenService>();

var app = builder.Build();

app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors(policy =>
{
    policy.AllowAnyHeader();
    policy.AllowAnyMethod();
    policy.AllowAnyOrigin();
});

app.MapControllers();

app.Run(url: "http://localhost:5139");
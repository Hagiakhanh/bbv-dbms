using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using OnlineStore.Repositories.Context;
using OnlineStore.Repositories.Store;
using OnlineStore.Repositories.Users;
using OnlineStore.Services.Auth;
using OnlineStore.Services.Tokens;
using OnlineStore.Services.Users;

namespace OnlineStore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add Controllers
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            // Configure Swagger with JWT Bearer Support
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Online Store API",
                    Version = "v1",
                    Description = "API for Customer Management & Administration Dashboard"
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Paste ONLY your JWT Access Token here (Do NOT include 'Bearer ' prefix, Swagger will add it automatically)."
                });

                options.AddSecurityRequirement((doc) => new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecuritySchemeReference("Bearer", doc),
                        new List<string>()
                    }
                });
            });

            // Configure JWT Authentication
            var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "SuperSecretKeyForOnlineStoreManagementApi2026!";
            var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "OnlineStoreAPI";
            var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "OnlineStoreClient";

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
                };
            });

            builder.Services.AddAuthorization();

            // Register Dependency Injection Services
            builder.Services.AddSingleton<JsonFileContext>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IStoreRepository, StoreRepository>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Online Store API v1");
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

            app.Run();
        }
    }
}

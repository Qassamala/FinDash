using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using FinDash.Data;
using FinDash.Services;
using FinDash.Config;

namespace FinDash
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var config = builder.Configuration;
            var services = builder.Services;

            // Add secretfile to config
            config.AddJsonFile("appsettings.Secret.json", optional: true, reloadOnChange: true);

            // Set the connectin string
            string connectionString = config.GetConnectionString("FinDashDbContext")!;

            // Add DbContext
            services.AddDbContext<FinDashDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            // Add JWT Authentication
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = config[ConfigurationKeys.JWTValidIssuer],
                    //ValidAudience = config["JwtSettings:ValidAudience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config[ConfigurationKeys.JWTSecretKey]!)),  // Retrieve the secret key for JWT and encode it
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                };
            });

            // Add services to the container.
            services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            // Adding PasswordService that generates salt and hashes passwords
            services.AddSingleton<PasswordService>();

            // Adding TokenService that handles token generation
            services.AddSingleton<TokenService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Authentication should happen first, then authorization
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
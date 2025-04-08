using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Notes_Back_CS.Connections.Configurations;
using Notes_Back_CS.Connections.Database;
using Notes_Back_CS.Extensions.Middlewares;
using Notes_Back_CS.Services.Tarefa;
using Notes_Back_CS.Services.Tarefa.Interface;
using Notes_Back_CS.Services.Usuario;
using Notes_Back_CS.Services.Usuario.Interface;
using Serilog;
using System.Text;

namespace Notes_Back_CS.Extensions
{
    public class NativeInjector
    {
        public static void RegisterServices(IConfiguration configuration, IServiceCollection services)
        {
            #region Logger

            var logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();

            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(logger: logger, dispose: true));

            #endregion Logger

            #region Services

            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<ITarefaService, TarefaService>();

            #endregion Services

            #region Others

            ServiceLocator.IncluirServico(services.BuildServiceProvider());

            #endregion Others
        }

        public static void RegisterBuild(WebApplicationBuilder builder)
        {
            #region Context

            builder.Services.AddDbContext<DatabaseContext>(opt =>
            {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("Database")).EnableSensitiveDataLogging();
            });
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

            #endregion Context

            #region Swagger

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerConfiguration();

            #endregion Swagger

            #region JWT

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                };
            });

            #endregion JWT

            #region CORS

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: "Origins",
                                  policy =>
                                  {
                                      policy.WithOrigins("http://localhost:4200",
                                                          "http://localhost",
                                                          "https://localhost:4200",
                                                          "https://localhost");
                                      policy.AllowAnyMethod()
                                            .AllowAnyHeader()
                                            .AllowAnyOrigin()
                                            .SetIsOriginAllowedToAllowWildcardSubdomains();
                                  });
            });

            #endregion CORS
        }

        public static void ConfigureApp(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.AddMiddlewares();
            app.UseCors("Origins");

            #region Files

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Files")),
                RequestPath = "/Files"
            });

            #endregion Files

            app.UseSwaggerConfiguration();

            #region Auth

            app.UseAuthentication();
            app.UseAuthorization();

            #endregion Auth
        }
    }

    public static class MiddlewareRegistrationExtension
    {
        public static void AddMiddlewares(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<ErrorMiddleware>();
        }
    }
}
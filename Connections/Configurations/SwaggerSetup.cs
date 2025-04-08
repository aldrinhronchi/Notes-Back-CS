using Microsoft.OpenApi.Models;

namespace Notes_Back_CS.Connections.Configurations
{
    public static class SwaggerSetup
    {
        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
            return services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = " API .Net Core",
                    Version = "v0.1",
                    Description = "API Base do Sistema de Notas de Aldrin 'Terore' Ronchi feitos em .Net",
                    Contact = new OpenApiContact
                    {
                        Name = "Aldrin",
                        Email = "work.aldrinronchi@gmail.com"
                    }
                });
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Insira o token de autenticação",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            },
                            Scheme = "oauth2",
                            Name ="Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<String>()
                    }
                });
            });
        }

        public static IApplicationBuilder UseSwaggerConfiguration(this IApplicationBuilder app)
        {
            return app.UseSwagger().UseSwaggerUI();
        }
    }
}
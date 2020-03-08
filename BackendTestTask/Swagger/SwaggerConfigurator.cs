using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

using BackendTestTask.Authorization;
using BackendTestTask.Authentification;
using System.Reflection;
using System.IO;
using System;

namespace BackendTestTask
{
    public static class SwaggerConfigurator
    {
        public static void ConfigureSwaggerFeature(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BackendTestTask", Version = "v1" });

                //Configure Swagger serucity 
                c.AddSecurityDefinition(ApiKeyConstants.HeaderName, new OpenApiSecurityScheme
                {

                    Description = "Api key needed to access the endpoints. ApiKey: 209e75ab-674b-41a3-92ea-eae383aba37d",
                    In = ParameterLocation.Header,
                    Name = ApiKeyConstants.HeaderName,
                    Type = SecuritySchemeType.ApiKey
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Name = ApiKeyConstants.HeaderName,
                            Type = SecuritySchemeType.ApiKey,
                            In = ParameterLocation.Header,
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = ApiKeyConstants.HeaderName },
                        },

                        new string[] {}
                    }

                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });


        }

        
    }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using BackendTestTask.Authorization;
using BackendTestTask.Automapper;
using BackendTestTask.DataAccess.Repository;
using BackendTestTask.DTO;
using BackendTestTask.Models;
using BackendTestTask.Extensions;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using BackendTestTask.Authentification;
using Serilog;

namespace BackendTestTask
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Add Custom Authentification settings
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = ApiKeyAuthenticationOptions.DefaultScheme;
                options.DefaultChallengeScheme = ApiKeyAuthenticationOptions.DefaultScheme;
            }).AddApiKeySupport(options => { });

            //Add Custom Authorization settings
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.OnlyAdmin, policy => policy.Requirements.Add(new OnlyAdminRequirement()));
            });
            //Add random generated Api Key in memory store as singleton service
            services.AddSingleton<IAuthorizationHandler, OnlyAdminAuthorizationHandler>();
            services.AddSingleton<IGetAllApiKeyQuery, InMemoryGetApiKeyQuery>();
            string connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<BlogPostContext>(options => options.UseSqlServer(connection));//Configure local sql database
            services.AddTransient<IRepository, PostRepository>();
            services.AddAutoMapper(typeof(Startup));//Add AutoMapper service
            services.AddMemoryCache();//Add server side caching service
            services.ConfigureSwaggerFeature();//Add Swaggger
            //Add Newtonsoft Json serialization
            services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore); ;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSerilogRequestLogging();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "BackendTestTask");
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

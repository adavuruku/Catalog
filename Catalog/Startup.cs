using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using Catalog.Repositories;
using Catalog.Services;
using Catalog.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Catalog
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
            //mongo db setting
            //anytime you see a guid make it a string
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            //anytime you see a date time make it a string
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));
            
            var mongoDbSettings = Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
            services.AddSingleton<IMongoClient>(serviceProvider =>
            {
                
                return new MongoClient(mongoDbSettings.ConnectionString);
            });
            
            //services.AddSingleton<Interface (service), Class Implementing it (Service Imple)>();
            services.AddSingleton<IItemsRepository, MongoDbItemsRepository>(); //non async
            
            services.AddScoped<IItemsRepositoryAsync, MongoDbItemsRepositoryAsync>(); //Async
            //mongo ends AddSingleton -> for the application, AddScoped -> for every request
            
            //services.AddSingleton<IItemsRepository, InMemoryItemsRepository>();
            
            services.AddControllers();
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "Catalog", Version = "v1"}); });
            //setting health check the add endpoints.MapHealthChecks() to configure method
           // services.AddHealthChecks(); //for just api setting
            //to include both api and mongo db
            // add this package -> dotnet add package AspNetCore.HealthChecks.MongoDb
            services.AddHealthChecks().AddMongoDb(mongoDbSettings.ConnectionString, 
                name:"mongoDb", 
                timeout:TimeSpan.FromSeconds(3),
                tags: new[] { "ready"});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog v1"));
            }

            //this is for http redirections
            if (env.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }
            

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //health checker for the restApi
                //check to see if api is down or fail connections to DB
                //Use the check to see if rest API is Alive / Healthy
                //setting (2) , setting one is in ConfigureServices
                //services.AddHealthChecks();
                endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
                {
                    Predicate = (check) => check.Tags.Contains("ready"),
                    ResponseWriter = async (context, report) =>
                    {
                        var result = JsonSerializer.Serialize(
                            new
                            {
                                status = report.Status.ToString(),
                                checks = report.Entries.Select(entry => new
                                {
                                    name = entry.Key,
                                    status = entry.Value.Status.ToString(),
                                    exception = entry.Value.Exception != null ? entry.Value.Exception.Message : "null",
                                    duration = entry.Value.Duration.ToString()
                                })
                            }
                        );
                        context.Response.ContentType = MediaTypeNames.Application.Json;
                        await context.Response.WriteAsync(result);
                    }
                });
                
                endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
                {
                    Predicate = (_) => false
                });
                
                
            });
        }
    }
}
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Raven.Client.Documents;

namespace Developer_Assessment
{
    namespace AddressApi
    {
        public class Startup
        {
            public Startup(IConfiguration configuration)
            {
                Configuration = configuration;
            }

            public IConfiguration Configuration { get; }

            public void ConfigureServices(IServiceCollection services)
            {
                services.AddControllers(); // Add the MVC framework for controllers

                services.AddSingleton<IDocumentStore>(new DocumentStore
                {
                    Urls = new[] { "http://localhost:8080" }, // Replace with your RavenDB server URL
                    Database = "YourDatabaseName", // Replace with your database name
                }.Initialize());
            }

            public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                else
                {
                    app.UseHsts();
                }

                app.UseRouting();

                // Add CORS policy if needed
                app.UseCors(options =>
                {
                    options.AllowAnyOrigin();
                    options.AllowAnyMethod();
                    options.AllowAnyHeader();
                });

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers(); // Map the controllers to handle HTTP requests
                });

                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var documentStore = serviceScope.ServiceProvider.GetRequiredService<IDocumentStore>();
                    documentStore.Initialize();
                }
            }
        }
    }

}

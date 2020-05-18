using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Http;
using IntelligentMission.Web.Services;
using IntelligentMission.Web.Models;
using Microsoft.ProjectOxford.Face;
using Microsoft.Azure.Documents.Client;
using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs;

namespace IntelligentMission.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfiguration>(this.Configuration);
            //services.AddOptions();
            //services.Configure<IMConfig>(this.Configuration.GetSection("IntelligentMissionConfig"));

            // Add framework services.
            services.AddMemoryCache();
            //services.AddMvc();
            services.AddMvc(options => {
                options.Filters.Add(new RequireHttpsAttribute());
            });
            services.ConfigurePOCO<IMConfig>(this.Configuration.GetSection("IntelligentMissionConfig"));

            services.AddAuthentication(
                SharedOptions => SharedOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme);

            // Set up DI
            services.AddTransient<INewsProviderClient, NewsProviderClient>();
            services.AddTransient<ITranslationApiClient, TranslationApiClient>();
            services.AddTransient<ITextApiClient, TextApiClient>();
            services.AddTransient<ISpeakerIdApiClient, SpeakerIdApiClient>();
            services.AddTransient<IStorageClient, StorageClient>();
            services.AddTransient<IImageManager, ImageManager>();
            services.AddTransient<IVideoManager, VideoManager>();

            services.AddTransient<ServiceFactory>();
            services.AddTransient<FaceApiClient>();
            services.AddTransient<VisionApiClient>();
            services.AddTransient<VideoApiClient>();
            services.AddTransient<IMDbRepository>();
            services.AddTransient<ImageAnalyzer>();
            services.AddTransient<PersonManager>();
            services.AddTransient<AudioManager>();
            services.AddTransient<BlobServiceClient>(p => p.GetService<ServiceFactory>().CreateBlobServiceClient2());
            services.AddTransient<BlobServiceClient>(p => p.GetService<ServiceFactory>().CreateBlobServiceClient());
            services.AddTransient<FaceServiceClient>(p => p.GetService<ServiceFactory>().CreateFaceServiceClient2());
            services.AddTransient<DocumentClient>(p => p.GetService<ServiceFactory>().CreateDocumentClient2());
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var initTask = serviceScope.ServiceProvider.GetService<IMDbRepository>().InitializeDatabase();
                initTask.Wait();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            
            app.UseStaticFiles();
            //app.UseStaticFiles(new StaticFileOptions()
            //{
            //    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"node_modules")),
            //    RequestPath = new PathString("/node_modules")
            //});

            app.UseCookieAuthentication();

            app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
            {
                ClientId = Configuration["Authentication:AzureAd:ClientId"],
                Authority = Configuration["Authentication:AzureAd:AADInstance"] + Configuration["Authentication:AzureAd:TenantId"],
                CallbackPath = Configuration["Authentication:AzureAd:CallbackPath"]
            });

            //app.UseMiddleware(typeof(ExceptionHandlingMiddleware));

            app.UseMvc(routes =>
            {
                //routes.MapRoute(
                //    name: "default",
                //    template: "{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "account-routes",
                    template: "account/{action=Index}/{id?}",
                    defaults: new { controller = "Account" }
                );
                routes.MapRoute(
                    name: "spa-fallback",
                    template: "{*url}",
                    defaults: new { controller = "Home", action = "Index" }
                );
            });


            app.UseNodeModules(env);

            //TODO: ensure DocDb collections existence
            
        }
    }
}

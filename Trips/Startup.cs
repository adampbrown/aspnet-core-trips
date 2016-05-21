namespace TheWorld
{
    using System.Net;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.AspNet.Authentication.Cookies;
    using Microsoft.AspNet.Builder;
    using Microsoft.AspNet.Hosting;
    using Microsoft.AspNet.Http;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.AspNet.Mvc.Rendering;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.PlatformAbstractions;
    using Newtonsoft.Json.Serialization;
    using TheWorld.Models;
    using TheWorld.Services;
    using TheWorld.ViewModels;

    /// <summary>
    /// The application startup class.
    /// </summary>
    public class Startup
    {
        public static IConfigurationRoot Configuration;

        /// <summary>
        /// Builds the application configuration from the application environment.
        /// </summary>
        /// <param name="appEnv">The <see cref="IApplicationEnvironment"/>.</param>
        public Startup(IApplicationEnvironment appEnv)
        {
            var builder = new ConfigurationBuilder()
              .SetBasePath(appEnv.ApplicationBasePath)
              .AddJsonFile("config.json")
              .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        /// <summary>
        /// Adds services to the container (called by the .net core runtime).
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(config =>
            {
#if !DEBUG
            config.Filters.Add(new RequireHttpsAttribute());
#endif
      })
            .AddJsonOptions(opt =>
            {
                opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

            services.AddIdentity<WorldUser, IdentityRole>(config =>
            {
                config.User.RequireUniqueEmail = true;
                config.Password.RequiredLength = 8;
                config.Cookies.ApplicationCookie.LoginPath = "/Auth/Login";
                config.Cookies.ApplicationCookie.Events = new CookieAuthenticationEvents()
                {
                    OnRedirectToLogin = ctx =>
                {
                    if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
                    {
                        ctx.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    }
                    else
                    {
                        ctx.Response.Redirect(ctx.RedirectUri);
                    }

                    return Task.FromResult(0);
                }
                };
            })
            .AddEntityFrameworkStores<WorldContext>();

            services.AddLogging();

            services.AddEntityFramework()
              .AddSqlServer()
              .AddDbContext<WorldContext>();

            services.AddScoped<CoordService>();
            services.AddTransient<WorldContextSeedData>();
            services.AddScoped<IWorldRepository, WorldRepository>();

#if DEBUG
            services.AddScoped<IMailService, MockMailService>();
#else
            services.AddScoped<IMailService, MailService>();
#endif
        }

        /// <summary>
        /// Configures the HTTP request pipeline (called by the .net core runtime).
        /// </summary>
        public async void Configure(IApplicationBuilder app,
          WorldContextSeedData seeder,
          ILoggerFactory loggerFactory,
          IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                loggerFactory.AddDebug(LogLevel.Information);
                app.UseDeveloperExceptionPage();
            }
            else
            {
                loggerFactory.AddDebug(LogLevel.Debug);
                app.UseExceptionHandler("/App/Error");
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseIdentity();

            Mapper.Initialize(config =>
            {
                config.CreateMap<Trip, TripViewModel>().ReverseMap();
                config.CreateMap<Stop, StopViewModel>().ReverseMap();
            });

            app.UseMvc(config =>
                {
                    config.MapRoute(
                        name: "Default",
                        template: "{controller}/{action}/{id?}",
                        defaults: new { controller = "App", action = "Index" });
                });

            await seeder.EnsureSeedDataAsync();
        }

        /// <summary>
        /// Entry point for the application.
        /// </summary>
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}

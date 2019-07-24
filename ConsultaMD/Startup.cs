using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConsultaMD.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.IO;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.HttpOverrides;
using WebEssentials.AspNetCore.Pwa;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace ConsultaMD
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public string os = Environment.OSVersion.Platform.ToString();

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString(os + "Connection")));
            services.AddDefaultIdentity<IdentityUser>()
                .AddDefaultUI(UIFramework.Bootstrap4)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddProgressiveWebApp(new PwaOptions()
            {
                RegisterServiceWorker = false
            });

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddViewLocalization()
                .AddDataAnnotationsLocalization();
            services.AddProgressiveWebApp();

            services.AddNodeServices(o =>
            {
                o.ProjectPath = "./";
            });

            services.Configure<RequestLocalizationOptions>(
                opts =>
                {
                    var supportedCultures = new List<CultureInfo>
                    {
                                    new CultureInfo("es"),
                                    new CultureInfo("en")
                    };

                    opts.DefaultRequestCulture = new RequestCulture("es");
                                // Formatting numbers, dates, etc.
                                opts.SupportedCultures = supportedCultures;
                                // UI strings that we have localized.
                                opts.SupportedUICultures = supportedCultures;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            var path = new List<string> { "lib", "cldr-data", "main" };

            var ch = os == "Win32NT" ? @"\" : "/";

            var di = new DirectoryInfo(Path.Combine(env.WebRootPath, string.Join(ch, path)));
            var supportedCultures = di.GetDirectories().Where(x => x.Name != "root").Select(x => new CultureInfo(x.Name)).ToList();
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(supportedCultures.FirstOrDefault(x => x.Name == "es")),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "Areas",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

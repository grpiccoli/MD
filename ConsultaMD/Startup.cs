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
using Microsoft.AspNetCore.Mvc.Infrastructure;
using ConsultaMD.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using AspNetCore.IServiceCollection.AddIUrlHelper;
using ConsultaMD.Models.Entities;
using Microsoft.AspNetCore.StaticFiles;
using ConsultaMD.Resources;
using WebMarkupMin.AspNetCore2;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Twilio;
using Microsoft.Extensions.FileProviders;
using ConsultaMD.Areas.Patients.Controllers;

namespace ConsultaMD
{
    public class Startup
    {
        private readonly string _os;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _os = Environment.OSVersion.Platform.ToString();
        }

    public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var accountSid = Configuration["Twilio:AccountSID"];
            var authToken = Configuration["Twilio:AuthToken"];
            //var accountSid = "AC3e87b4b00a0e460583a7e8b77bca6620";
            //var authToken = "837533360893e16b150eca01c03c811f";
            TwilioClient.Init(accountSid, authToken);

            services.Configure<TwilioVerifySettings>(Configuration.GetSection("Twilio"));

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false; //!!!!CAMBIAR A TRUE
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.Cookie.Name = "ConsultaMD";
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.LoginPath = "/Identity/Account/Login";
                // ReturnUrlParameter requires 
                //using Microsoft.AspNetCore.Authentication.Cookies;
                options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                options.SlidingExpiration = true;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString($"{_os}Connection"),
                    sqlServerOptions => sqlServerOptions.CommandTimeout(100)));

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = false; //!!!!CAMBIAR A TRUE
                options.SignIn.RequireConfirmedPhoneNumber = false; //!!!!CAMBIAR A TRUE
                options.User.AllowedUserNameCharacters =
                            "K0123456789-.";
                options.User.RequireUniqueEmail = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                // Default Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            })
                .AddDefaultUI(UIFramework.Bootstrap4)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddErrorDescriber<SpanishIdentityErrorDescriber>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(o =>
                {
                    o.LoginPath = new PathString("/login");
                    o.AccessDeniedPath = new PathString("/Account/AccessDenied");
                    o.LogoutPath = new PathString("/logout");
                })
                .AddFacebook(facebookOptions => 
                {
                    facebookOptions.AppId = "461203471369329";
                    facebookOptions.AppSecret = "e9c5b1f949e9dfbc26afe2017ea8bf75";
                })
                .AddGoogle(options =>
                {
                    IConfigurationSection googleAuthNSection =
                        Configuration.GetSection("Authentication:Google");

                    options.ClientId = "1062549201152-14oa75aaad7ufn8j9s3jm85m92bdts5e.apps.googleusercontent.com";
                    options.ClientSecret = "n5fwgUhQXbuSGMp09tLAiHVk";
                });

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddProgressiveWebApp();

            services.AddTransient<IEmailSender, EmailSender>();

            services.AddScoped<IViewRenderService, ViewRenderService>();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            // Add WebMarkupMin services.
            services.AddWebMarkupMin()
                .AddHtmlMinification()
                .AddXmlMinification()
                .AddHttpCompression();

            Libman.LoadJson();
            Bundler.LoadJson();
            Actions.LoadJson();
            //WebpackChunkNamer.Init();

            services.AddMvc(o => {
                o.ModelMetadataDetailsProviders.Add(
                    new LocalizedValidationMetadataProvider(
                        "ConsultaMD.Resources.ValidationMessages", typeof(ValidationMessages)
                    ));
                //var underlyingModelBinder = o.ModelBinderProviders.FirstOrDefault(x => x.GetType() == typeof(BodyModelBinderProvider));
                //if (underlyingModelBinder == null) return;
                //var index = o.ModelBinderProviders.IndexOf(underlyingModelBinder);
                //o.ModelBinderProviders.Insert(index, new CustomValidationModelBinderProvider(underlyingModelBinder));
            })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddViewLocalization()
                .AddDataAnnotationsLocalization();

            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(60);
            });

            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                //options.HttpsPort = 5101;
            });

            services.AddNodeServices(o =>
            {
                o.ProjectPath = "./";
            });

            services.Configure<AuthMessageSenderOptions>(Configuration);

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
            services.AddUrlHelper();
            services.AddSignalR(options => {
                options.EnableDetailedErrors = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (app == null || env == null) return;
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(
                        Path.Combine(Directory.GetCurrentDirectory(), "src")),
                    RequestPath = "/src"
                });
                //app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                //{
                //    ProjectPath = env.ContentRootPath,
                //    HotModuleReplacement = true
                //});
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                app.UseWebMarkupMin();
            }

            app.UseDefaultImage(defaultImagePath: Configuration.GetSection($"{_os}defaultImagePath").Value);

            var path = new List<string> { "wwwroot", "lib", "cldr-data", "main" };

            var ch = _os == "Win32NT" ? @"\" : "/";

            var di = new DirectoryInfo(Path.Combine(env.ContentRootPath, string.Join(ch, path)));
            var supportedCultures = di.GetDirectories().Where(x => x.Name != "root").Select(x => new CultureInfo(x.Name)).ToList();
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(supportedCultures.FirstOrDefault(x => x.Name == "es")),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });
            app.UseHttpsRedirection();
            FileExtensionContentTypeProvider provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".webmanifest"] = "application/manifest+json";
            app.UseStaticFiles(new StaticFileOptions()
            {
                ContentTypeProvider = provider
            });
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

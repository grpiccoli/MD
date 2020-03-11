using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
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
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using ConsultaMD.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using AspNetCore.IServiceCollection.AddIUrlHelper;
using ConsultaMD.Models.Entities;
using Microsoft.AspNetCore.StaticFiles;
using ConsultaMD.Resources;
using WebMarkupMin.AspNetCore2;
using Twilio;
using Microsoft.Extensions.FileProviders;
using ConsultaMD.Hubs;

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
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(
                    Configuration.GetConnectionString($"{_os}Connection"),
                    sqlServerOptions => sqlServerOptions.CommandTimeout(10000));
            });

            //services.AddLogging(loggingBuilder =>
            //{
            //    loggingBuilder.AddConsole();
            //    loggingBuilder.AddDebug();
            //});

            services.AddScoped<ISeed, SeedService>();
            services.AddHostedService<SeedBackground>();

            var antiCaptchaKey = Configuration["AntiCaptcha"];
            AntiCaptchaClient.Init(antiCaptchaKey);

            //services.AddHostedService<FonasaBackground>();
            services.AddScoped<IPuppet, PuppetService>();

            services.AddScoped<ISuperSaludService, SuperSaludService>();
            services.Configure<RegCivilSettings>(o =>
            {
                o.Url = new Uri("https://portal.sidiv.registrocivil.cl/usuarios-portal/pages/DocumentRequestStatus.xhtml");
            });
            services.AddScoped<IRegCivil, RegCivilService>();
            //services.AddHostedService<RegCivilBackground>();

            services.AddScoped<IFonasa, FonasaService>();

            services.Configure<FlowSettings>(o =>
            {
                var dev = true;
                var flowEnv = dev ? "Sandbox" : "Production";
                var preffix = dev ? "sandbox" : "www";
                o.ApiKey = Configuration[$"Flow:{flowEnv}:ApiKey"];
                o.SecretKey = Configuration[$"Flow:{flowEnv}:SecretKey"];
                o.Currency = "UF";
                o.EndPoint = new Uri($"https://{preffix}.flow.cl/api");
            });
            services.AddScoped<IFlow, FlowService>();

            services.AddScoped<IRedirect, RedirectService>();
            services.AddScoped<IMedium, MediumService>();

            var accountSid = Configuration["Twilio:AccountSID"];
            var authToken = Configuration["Twilio:AuthToken"];
            TwilioClient.Init(accountSid, authToken);
            services.Configure<TwilioVerifySettings>(Configuration.GetSection("Twilio"));

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true; //!!!!CAMBIAR A TRUE
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

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = false; //!!!!CAMBIAR A TRUE
                options.SignIn.RequireConfirmedPhoneNumber = false; //!!!!CAMBIAR A TRUE
                options.User.AllowedUserNameCharacters = "K0123456789-.";
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
                .AddDefaultUI()
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
                    facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
                    facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
                })
                .AddGoogle(options =>
                {
                    IConfigurationSection googleAuthNSection =
                        Configuration.GetSection("Authentication:Google");

                    options.ClientId = Configuration["Authentication:Google:ClientId"];
                    options.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                });

            services.AddLocalization(options => options.ResourcesPath = "Resources");

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
            services.AddScoped<IMIP, MIPService>();
            services.AddScoped<IEvent, EventService>();
            //WebpackChunkNamer.Init();

            services.AddProgressiveWebApp();

            services.AddMvc(o => 
                o.ModelMetadataDetailsProviders.Add(
                    new LocalizedValidationMetadataProvider(
                        "ConsultaMD.Resources.ValidationMessages", typeof(ValidationMessages)
                    ))
                //var underlyingModelBinder = o.ModelBinderProviders.FirstOrDefault(x => x.GetType() == typeof(BodyModelBinderProvider));
                //if (underlyingModelBinder == null) return;
                //var index = o.ModelBinderProviders.IndexOf(underlyingModelBinder);
                //o.ModelBinderProviders.Insert(index, new CustomValidationModelBinderProvider(underlyingModelBinder));
            )
                .SetCompatibilityVersion(CompatibilityVersion.Latest)
                .AddViewLocalization()
                .AddDataAnnotationsLocalization()
                .AddNewtonsoftJson();

            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(60);
            });

            services.AddHttpsRedirection(options =>
            options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect);

            //services.AddNodeServices(o => o.InvocationTimeoutMilliseconds = 600_000);

            services.Configure<AuthMessageSenderOptions>(Configuration);

            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("es-CL");
            services.Configure<RequestLocalizationOptions>(
                opts =>
                {
                    var supportedCultures = new List<CultureInfo>
                    {
                                    new CultureInfo("es-CL"),
                                    new CultureInfo("es"),
                                    new CultureInfo("en")
                    };

                    opts.DefaultRequestCulture = new RequestCulture("es-CL");
                    // Formatting numbers, dates, etc.
                    opts.SupportedCultures = new List<CultureInfo> { new CultureInfo("es-CL") };
                    // UI strings that we have localized.
                    opts.SupportedUICultures = supportedCultures;
                });
            services.AddUrlHelper();
            services.AddSignalR(options => {
                options.EnableDetailedErrors = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
                app.UseSpaStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(
                        Path.Combine(Directory.GetCurrentDirectory(), "node_modules")),
                    RequestPath = "/node_modules"
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
            app.UseRouting();

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
            app.UseAuthorization();

            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllers().RequireAuthorization();
                endpoints.MapRazorPages().RequireAuthorization();
                endpoints.MapHub<FeedBackHub>("/feedbackHub");
                // need route and attribute on controller: [Area("Blogs")]
                endpoints.MapControllerRoute(name: "mvcAreaRoute",
                                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}")
                .RequireAuthorization();
                // default route for non-areas
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{area=Patients}/{controller=Search}/{action=Map}")
                .RequireAuthorization();
                //endpoints.MapDefaultControllerRoute().RequireAuthorization();
                //endpoints.MapControllerRoute(
                //    name: "default",
                //    pattern: "{controller=Home}/{action=Index}/{id?}").RequireAuthorization();
                //endpoints.MapControllerRoute(
                //    name: "default",
                //    pattern: "{area:exists=Patients}/{controller=Home}/{action=Index}/{id?}").RequireAuthorization();
                //endpoints.MapAreaControllerRoute(
                //    name: "users",
                //    areaName: "Patients",
                //    pattern: "{area}/{controller}/{action}/{id?}",
                //    defaults: new { area = "Patients", controller = "Search", action = "Map" })
                //.RequireAuthorization();
                //endpoints.MapAreaControllerRoute(
                //    name: "doctors",
                //    areaName: "MDs",
                //    pattern: "{area}/{controller}/{action}/{id?}",
                //    defaults: new { area = "MDs", controller = "MDash", action = "Agenda" })
                //.RequireAuthorization();
            });
        }
    }
}

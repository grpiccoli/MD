using System;
using System.IO;
using System.Linq;
using System.Net;
using ConsultaMD.Data;
using ConsultaMD.Models.Entities;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ConsultaMD
{
    public class Program
    {
        public static IConfigurationRoot Configuration;
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDbContext>();
                var normalizer = services.GetRequiredService<ILookupNormalizer>();
                var env = services.GetRequiredService<IHostingEnvironment>();

                try
                {
                    string os = Environment.OSVersion.Platform.ToString();
                    string connection = Configuration.GetConnectionString($"{os}Connection");

                    //Localities
                    //var conf = services.GetRequiredService<IConfiguration>();
                    //var connection = conf.GetConnectionString(os + "Connection");
                    //REMEMBER!!!!!! ALWAYS ENCODE USC-2 LE BOM
                    var tsvPath = Path.Combine(env.ContentRootPath, "Data", "Locality");
                    if (!context.Localities.Any())
                        BulkInsert.RunSql<Locality>(tsvPath, connection);
                    if (!context.AreaCodes.Any())
                        BulkInsert.RunSql<AreaCode>(tsvPath, connection);

                    tsvPath = Path.Combine(env.ContentRootPath, "Data", "MD");
                    if (!context.People.Any())
                        BulkInsert.RunSql<Person>(tsvPath, connection);

                    UserInitializer.Initialize(context, normalizer);
                    var adminId = context.ApplicationUsers.Where(u => u.Email == "contacto@epicsolutions.cl").SingleOrDefault().Id;

                    if (!context.DigitalSignatures.Any())
                        BulkInsert.RunSql<DigitalSignature>(tsvPath, connection);
                    if (!context.Doctors.Any())
                        BulkInsert.RunSql<Doctor>(tsvPath, connection);
                    if (!context.InsuranceLocations.Any())
                        BulkInsert.RunSql<InsuranceLocation>(tsvPath, connection);
                    if (!context.MedicalAttentionMediums.Any())
                        BulkInsert.RunSql<MedicalAttentionMedium>(tsvPath, connection);
                    if (!context.MediumDoctors.Any())
                        BulkInsert.RunSql<MediumDoctor>(tsvPath, connection);
                    if (!context.Places.Any())
                        BulkInsert.RunSql<Place>(tsvPath, connection);
                    if (!context.Agenda.Any())
                        BulkInsert.RunSql<Agenda>(tsvPath, connection);
                    if (!context.TimeSlots.Any())
                        BulkInsert.RunSql<TimeSlot>(tsvPath, connection);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "There has been an error while seeding the database.");
                }
            }
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .ConfigureKestrel(options =>
            {
                string os = Environment.OSVersion.Platform.ToString();

                options.Limits.MaxConcurrentConnections = 200;
                options.Limits.MaxConcurrentUpgradedConnections = 200;
                //options.Limits.MaxRequestBodySize = 20_000_000;
                //options.Limits.MinRequestBodyDataRate =
                //    new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
                //options.Limits.MinResponseDataRate =
                //    new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
                options.Listen(IPAddress.Parse("127.0.0.10"), 5100);
                options.Listen(IPAddress.Parse("127.0.0.10"), 5101,
                    listenOptions =>
                {
                    listenOptions.UseHttps(os == "Win32NT" ?
                        Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "sslforfree/consultamd.pfx") : "/media/guillermo/WD3DNAND-SSD-1TB/certs/consultamd.pfx", "34#$ERer");
                });
            })
                .UseStartup<Startup>();
    }
}

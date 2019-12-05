using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using ConsultaMD.Data;
using ConsultaMD.Models.Entities;
using ConsultaMD.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace ConsultaMD
{
    public static class Program
    {
        private static IConfigurationRoot Configuration;
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
                var eventService = services.GetRequiredService<IEvent>();
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
                    //if (!context.AreaCodes.Any())
                    //    BulkInsert.RunSql<AreaCode>(tsvPath, connection);
                    //if (!context.AreaCodeProvinces.Any())
                    //    BulkInsert.RunSql<AreaCodeProvince>(tsvPath, connection);

                    tsvPath = Path.Combine(env.ContentRootPath, "Data", "MD");
                    if (!context.People.Any())
                        BulkInsert.RunSql<Person>(tsvPath, connection);

                    UserInitializer.Initialize(context, normalizer);
                    var adminId = context.ApplicationUsers.Where(u => u.Email == "contacto@epicsolutions.cl").SingleOrDefault().Id;

                    //if (!context.DigitalSignatures.Any())
                    //    BulkInsert.RunSql<DigitalSignature>(tsvPath, connection);
                    if (!context.Doctors.Any())
                        BulkInsert.RunSql<Doctor>(tsvPath, connection);
                    if (!context.Specialties.Any())
                        BulkInsert.RunSql<Specialty>(tsvPath, connection);
                    if (!context.DoctorSpecialties.Any())
                        BulkInsert.RunSql<DoctorSpecialty>(tsvPath, connection);
                    if (!context.InsuranceLocations.Any())
                        BulkInsert.RunSql<InsuranceAgreement>(tsvPath, connection);
                    if (!context.InsuranceLocations.Any())
                        BulkInsert.RunSql<InsuranceLocation>(tsvPath, connection);
                    if (!context.MedicalAttentionMediums.Any())
                        BulkInsert.RunSql<MedicalAttentionMedium>(tsvPath, connection);
                    if (!context.MediumDoctors.Any())
                        BulkInsert.RunSql<MediumDoctor>(tsvPath, connection);
                    if (!context.Places.Any())
                        BulkInsert.RunSql<Place>(tsvPath, connection);

                    if (!context.AgendaEvents.Any())
                        eventService.AddEvents(new List<AgendaEvent> { 
                            new AgendaEvent {
                                MediumDoctorId = 1,
                                StartDateTime = new DateTime(2019,11,15,11,30,0),
                                EndDateTime = new DateTime(2019,12,13,17,15,0),
                                Duration = new TimeSpan(0,10,0),
                                Frequency = 1,
                                daysOfWeek = new List<DayOfWeek>{ 
                                    DayOfWeek.Monday,
                                    DayOfWeek.Tuesday,
                                    DayOfWeek.Wednesday
                                }
                            },
                            new AgendaEvent {
                                MediumDoctorId = 5,
                                StartDateTime = new DateTime(2019,11,15,11,30,0),
                                EndDateTime = new DateTime(2019,12,13,17,15,0),
                                Duration = new TimeSpan(0,10,0),
                                Frequency = 1,
                                daysOfWeek = new List<DayOfWeek>{
                                    DayOfWeek.Thursday,
                                    DayOfWeek.Friday,
                                    DayOfWeek.Monday
                                }
                            },
                            new AgendaEvent {
                                MediumDoctorId = 6,
                                StartDateTime = new DateTime(2019,11,15,15,50,0),
                                EndDateTime = new DateTime(2019,12,17,18,30,0),
                                Duration = new TimeSpan(0,10,0),
                                Frequency = 1,
                                daysOfWeek = new List<DayOfWeek>{
                                    DayOfWeek.Tuesday,
                                    DayOfWeek.Wednesday,
                                    DayOfWeek.Thursday
                                }
                            },
                            new AgendaEvent {
                                MediumDoctorId = 8,
                                StartDateTime = new DateTime(2019,11,15,11,30,0),
                                EndDateTime = new DateTime(2019,12,13,17,15,0),
                                Duration = new TimeSpan(0,10,0),
                                Frequency = 1,
                                daysOfWeek = new List<DayOfWeek>{
                                    DayOfWeek.Friday,
                                    DayOfWeek.Monday,
                                    DayOfWeek.Tuesday
                                }
                            },
                            new AgendaEvent {
                                MediumDoctorId = 11,
                                StartDateTime = new DateTime(2019,11,18,10,0,0),
                                EndDateTime = new DateTime(2019,11,18,12,30,0),
                                Duration = new TimeSpan(0,10,0),
                                Frequency = 1,
                                daysOfWeek = new List<DayOfWeek>{
                                    DayOfWeek.Tuesday,
                                    DayOfWeek.Wednesday,
                                    DayOfWeek.Thursday
                                }
                            },
                            new AgendaEvent {
                                MediumDoctorId = 12,
                                StartDateTime = new DateTime(2019,11,15,11,30,0),
                                EndDateTime = new DateTime(2019,11,15,17,15,0),
                                Duration = new TimeSpan(0,10,0),
                                Frequency = 1,
                                daysOfWeek = new List<DayOfWeek>{
                                    DayOfWeek.Friday,
                                    DayOfWeek.Monday,
                                    DayOfWeek.Tuesday
                                }
                            },
                            new AgendaEvent {
                                MediumDoctorId = 13,
                                StartDateTime = new DateTime(2019,11,15,9,30,0),
                                EndDateTime = new DateTime(2019,11,15,13,0,0),
                                Duration = new TimeSpan(0,10,0),
                                Frequency = 1,
                                daysOfWeek = new List<DayOfWeek>{
                                    DayOfWeek.Wednesday,
                                    DayOfWeek.Thursday,
                                    DayOfWeek.Friday
                                }
                            },
                            new AgendaEvent {
                                MediumDoctorId = 15,
                                StartDateTime = new DateTime(2019,11,19,9,0,0),
                                EndDateTime = new DateTime(2019,11,19,12,30,0),
                                Duration = new TimeSpan(0,10,0),
                                Frequency = 1,
                                daysOfWeek = new List<DayOfWeek>{
                                    DayOfWeek.Monday,
                                    DayOfWeek.Tuesday,
                                    DayOfWeek.Wednesday
                                }
                            },
                            new AgendaEvent {
                                MediumDoctorId = 16,
                                StartDateTime = new DateTime(2019,11,15,9,0,0),
                                EndDateTime = new DateTime(2019,11,15,18,10,0),
                                Duration = new TimeSpan(0,10,0),
                                Frequency = 1,
                                daysOfWeek = new List<DayOfWeek>{
                                    DayOfWeek.Thursday,
                                    DayOfWeek.Friday,
                                    DayOfWeek.Monday
                                }
                            },
                            new AgendaEvent {
                                MediumDoctorId = 17,
                                StartDateTime = new DateTime(2019,11,18,15,0,0),
                                EndDateTime = new DateTime(2019,11,18,18,10,0),
                                Duration = new TimeSpan(0,10,0),
                                Frequency = 1,
                                daysOfWeek = new List<DayOfWeek>{
                                    DayOfWeek.Thursday,
                                    DayOfWeek.Friday,
                                    DayOfWeek.Monday
                                }
                            },
                            new AgendaEvent {
                                MediumDoctorId = 24,
                                StartDateTime = new DateTime(2019,11,15,11,30,0),
                                EndDateTime = new DateTime(2019,12,13,17,15,0),
                                Duration = new TimeSpan(0,10,0),
                                Frequency = 1,
                                daysOfWeek = new List<DayOfWeek>{
                                    DayOfWeek.Thursday,
                                    DayOfWeek.Friday,
                                    DayOfWeek.Monday
                                }
                            },
                            new AgendaEvent {
                                MediumDoctorId = 25,
                                StartDateTime = new DateTime(2019,11,15,11,30,0),
                                EndDateTime = new DateTime(2019,12,13,17,15,0),
                                Duration = new TimeSpan(0,10,0),
                                Frequency = 1,
                                daysOfWeek = new List<DayOfWeek>{
                                    DayOfWeek.Thursday,
                                    DayOfWeek.Friday,
                                    DayOfWeek.Monday
                                }
                            },
                            new AgendaEvent {
                                MediumDoctorId = 26,
                                StartDateTime = new DateTime(2019,11,15,11,30,0),
                                EndDateTime = new DateTime(2019,12,13,17,15,0),
                                Duration = new TimeSpan(0,10,0),
                                Frequency = 1,
                                daysOfWeek = new List<DayOfWeek>{
                                    DayOfWeek.Thursday,
                                    DayOfWeek.Friday,
                                    DayOfWeek.Monday
                                }
                            }
                        }).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger>();
                    var localizer = services.GetRequiredService<IStringLocalizer>();
                    logger.LogError(ex, localizer["There has been an error while seeding the database."]);
                    throw;
                }
            }
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .ConfigureLogging(logging => 
            {
                logging.ClearProviders();
                logging.AddConsole();
            })
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

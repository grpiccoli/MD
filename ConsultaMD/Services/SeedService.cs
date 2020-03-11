using ConsultaMD.Data;
using ConsultaMD.Extensions;
using ConsultaMD.Models.Entities;
using ConsultaMD.Models.VM;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Pluralize.NET.Core;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ConsultaMD.Services
{
    public class SeedService : ISeed
    {
        private readonly ILogger _logger;
        private readonly IStringLocalizer _localizer;
        public IConfiguration Configuration { get; }
        private readonly IWebHostEnvironment _environment;
        private readonly string _os;
        private readonly string _conn;
        private readonly ApplicationDbContext _context;
        private readonly IEvent _events;
        private readonly ILookupNormalizer _normalizer;
        //private readonly IUser _user;
        //private int executionCount = 0;
        //private readonly Bulk _bulk;
        //private readonly string _conn;
        //private Timer _timer;
        public SeedService(
            ILogger<SeedService> logger,
            IStringLocalizer<SeedService> localizer,
            IConfiguration configuration,
            IWebHostEnvironment environment,
            ApplicationDbContext context,
            IEvent events,
            ILookupNormalizer normalizer
            //IUser user
            //Bulk bulk
            )
        {
            _logger = logger;
            _localizer = localizer;
            Configuration = configuration;
            _environment = environment;
            _os = Environment.OSVersion.Platform.ToString();
            _conn = Configuration.GetConnectionString($"{_os}Connection");
            _context = context;
            _events = events;
            _normalizer = normalizer;
            //_user = user;
            //_bulk = bulk;
        }
        public async Task Seed()
        {
            try
            {
                await AddProcedure().ConfigureAwait(false);
                //Localities
                //REMEMBER!!!!!! ALWAYS ENCODE USC-2 LE BOM
                var tsvPath = Path.Combine(_environment.ContentRootPath, "Data", "Locality");
                if (!_context.Localities.Any())
                    await Insert<Locality>(tsvPath).ConfigureAwait(false);
                //if (!context.AreaCodes.Any())
                //    BulkInsert.RunSql<AreaCode>(tsvPath, connection);
                //if (!context.AreaCodeProvinces.Any())
                //    BulkInsert.RunSql<AreaCodeProvince>(tsvPath, connection);

                tsvPath = Path.Combine(_environment.ContentRootPath, "Data", "MD");
                if (!_context.People.Any())
                    await Insert<Person>(tsvPath).ConfigureAwait(false);

                await Users().ConfigureAwait(false);
                //if (!context.DigitalSignatures.Any())
                //    BulkInsert.RunSql<DigitalSignature>(tsvPath, connection);
                if (!_context.Doctors.Any())
                    await Insert<Doctor>(tsvPath).ConfigureAwait(false);
                if (!_context.Specialties.Any())
                    await Insert<Specialty>(tsvPath).ConfigureAwait(false);
                if (!_context.DoctorSpecialties.Any())
                    await Insert<DoctorSpecialty>(tsvPath).ConfigureAwait(false);
                if (!_context.InsuranceLocations.Any())
                    await Insert<InsuranceAgreement>(tsvPath).ConfigureAwait(false);
                if (!_context.InsuranceLocations.Any())
                    await Insert<InsuranceLocation>(tsvPath).ConfigureAwait(false);
                if (!_context.MedicalAttentionMediums.Any())
                    await Insert<MedicalAttentionMedium>(tsvPath).ConfigureAwait(false);
                if (!_context.MediumDoctors.Any())
                    await Insert<MediumDoctor>(tsvPath).ConfigureAwait(false);
                if (!_context.Places.Any())
                    await Insert<Place>(tsvPath).ConfigureAwait(false);

                var today = DateTime.Now.AddDays(5);
                var ts10 = new TimeSpan(0, 10, 0);

                if (!_context.AgendaEvents.Any())
                    await _events.AddRange(new List<AgendaEvent> {
                            new AgendaEvent {
                                MediumDoctorId = 1,
                                StartDateTime = today.AddHours(11).AddMinutes(30),
                                EndDateTime = today.AddDays(28).AddHours(17).AddMinutes(15),
                                Duration = ts10,
                                Frequency = 1,
                                Days = 123
                            },
                            new AgendaEvent {
                                MediumDoctorId = 5,
                                StartDateTime = today.AddHours(11).AddMinutes(30),
                                EndDateTime = today.AddDays(28).AddHours(17).AddMinutes(15),
                                Duration = ts10,
                                Frequency = 1,
                                Days = 234
                            },
                            new AgendaEvent {
                                MediumDoctorId = 6,
                                StartDateTime = today.AddHours(15).AddMinutes(50),
                                EndDateTime = today.AddDays(32).AddHours(18).AddMinutes(30),
                                Duration = ts10,
                                Frequency = 1,
                                Days = 345
                            },
                            new AgendaEvent {
                                MediumDoctorId = 8,
                                StartDateTime = today.AddHours(11).AddMinutes(30),
                                EndDateTime = today.AddDays(28).AddHours(17).AddMinutes(15),
                                Duration = ts10,
                                Frequency = 1,
                                Days = 451
                            },
                            new AgendaEvent {
                                MediumDoctorId = 11,
                                StartDateTime = today.AddDays(8).AddHours(10),
                                EndDateTime = today.AddDays(8).AddHours(12).AddMinutes(30),
                                Duration = ts10,
                                Frequency = 1,
                                Days = 512
                            },
                            new AgendaEvent {
                                MediumDoctorId = 12,
                                StartDateTime = today.AddHours(11).AddMinutes(30),
                                EndDateTime = today.AddHours(17).AddMinutes(15),
                                Duration = ts10,
                                Frequency = 1,
                                Days = 123
                            },
                            new AgendaEvent {
                                MediumDoctorId = 13,
                                StartDateTime = today.AddHours(9).AddMinutes(30),
                                EndDateTime = today.AddHours(13),
                                Duration = ts10,
                                Frequency = 1,
                                Days = 234
                            },
                            new AgendaEvent {
                                MediumDoctorId = 15,
                                StartDateTime = today.AddDays(9).AddHours(9),
                                EndDateTime = today.AddDays(9).AddHours(12).AddMinutes(30),
                                Duration = ts10,
                                Frequency = 1,
                                Days = 345
                            },
                            new AgendaEvent {
                                MediumDoctorId = 16,
                                StartDateTime = today.AddHours(9),
                                EndDateTime = today.AddHours(18).AddMinutes(10),
                                Duration = ts10,
                                Frequency = 1,
                                Days = 451
                            },
                            new AgendaEvent {
                                MediumDoctorId = 17,
                                StartDateTime = today.AddDays(8).AddHours(15),
                                EndDateTime = today.AddDays(8).AddHours(18).AddMinutes(10),
                                Duration = ts10,
                                Frequency = 1,
                                Days = 512
                            },
                            new AgendaEvent {
                                MediumDoctorId = 24,
                                StartDateTime = today.AddDays(28).AddHours(10).AddMinutes(30),
                                EndDateTime = today.AddDays(28).AddHours(16).AddMinutes(30),
                                Duration = ts10,
                                Frequency = 1,
                                Days = 123
                            },
                            new AgendaEvent {
                                MediumDoctorId = 25,
                                StartDateTime = today.AddDays(28).AddHours(10).AddMinutes(30),
                                EndDateTime = today.AddDays(28).AddHours(16).AddMinutes(30),
                                Duration = ts10,
                                Frequency = 1,
                                Days = 234
                            },
                            new AgendaEvent {
                                MediumDoctorId = 26,
                                StartDateTime = today.AddHours(10).AddMinutes(30),
                                EndDateTime = today.AddHours(16).AddMinutes(30),
                                Duration = ts10,
                                Frequency = 1,
                                Days = 345
                            }
                        }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer["There has been an error while seeding the database."]);
                throw;
            }
        }
        public async Task AddProcedure()
        {
            string query = "select * from sysobjects where type='P' and name='BulkInsert'";
            var sp = @"CREATE PROCEDURE BulkInsert(@TableName NVARCHAR(50), @Tsv NVARCHAR(100))
AS
BEGIN 
DECLARE @SQLSelectQuery NVARCHAR(MAX)=''
SET @SQLSelectQuery = 'BULK INSERT ' + @TableName + ' FROM ' + QUOTENAME(@Tsv) + ' WITH (DATAFILETYPE=''widechar'')'
  exec(@SQLSelectQuery)
END";
            bool spExists = false;
            using SqlConnection connection = new SqlConnection(_conn);
            using SqlCommand command = new SqlCommand
            {
                Connection = connection,
                CommandText = query
            };
            connection.Open();
            using (SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
            {
                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    spExists = true;
                    break;
                }
            }
            if (!spExists)
            {
                command.CommandText = sp;
                using SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    spExists = true;
                    break;
                }
            }
            connection.Close();
        }
        public async Task Insert<TSource>(string path)
        {
            var name = new Pluralizer().Pluralize(typeof(TSource).ToString().Split(".").Last());
            _context.Database.SetCommandTimeout(10000);
            var tsv = Path.Combine(path, $"{name}.tsv");
            var tmp = Path.Combine(Path.GetTempPath(), $"{name}.tsv");
            File.Copy(tsv, tmp, true);
            await _context.Database
                .ExecuteSqlInterpolatedAsync($"BulkInsert {"dbo."+name}, {tmp}")
                .ConfigureAwait(false);
            //await _context.Database
            //    .ExecuteSqlCommandAsync($"BulkInsert @p0, @p1;", tableName, tmp)
            //    .ConfigureAwait(false);
            File.Delete(tmp);
            return;
        }
        public async Task Users()
        {
            //using (var transaction = _context.Database.BeginTransaction())
            using var roleStore = new RoleStore<ApplicationRole>(_context);
            using var userStore = new UserStore<ApplicationUser>(_context);
            if (!_context.ApplicationUserRoles.Any())
            {
                if (!_context.Users.Any())
                {
                    if (!_context.ApplicationRoles.Any())
                    {
                        var applicationRoles = new List<ApplicationRole>();
                        foreach (var item in RoleData.ApplicationRoles)
                        {
                            applicationRoles.Add(
                                new ApplicationRole
                                {
                                    CreatedDate = DateTime.Now,
                                    Name = item,
                                    Description = "",
                                    NormalizedName = item.ToLower(new CultureInfo("es-CL"))
                                });
                        };

                        foreach (var role in applicationRoles)
                        {
                            await _context.ApplicationRoles.AddAsync(role).ConfigureAwait(false);
                        }
                        await _context.SaveChangesAsync().ConfigureAwait(false);
                    }

                    var users = new UserInitializerVM[]
                    {
                                new UserInitializerVM
                                {
                                    Names = "GUILLERMO ANTONIO",
                                    LastF = "RODRÍGUEZ",
                                    LastM = "PICCOLI",
                                    RUN = 16124902,
                                    Carnet = 519194461,
                                    Email = "contacto@epicsolutions.cl",
                                    Role = "Administrator",
                                    Key = "test2019",
                                    Claim = "webmaster",
                                    Insurance = InsuranceData.Insurance.Fonasa,
                                    Birth = new DateTime(1985,6,7),
                                    Sex = true,
                                    Tramo = Tramo.B
                                },
                                new UserInitializerVM
                                {
                                    Names = "JORGE ALEJANDRO",
                                    LastF = "MUNÓZ",
                                    LastM = "BRAND",
                                    RUN = 11927437,
                                    Carnet = 111111111,
                                    Email = "grpiccoli@gmail.com",
                                    Key = "guillermo",
                                    Insurance = InsuranceData.Insurance.Banmedica,
                                    Birth = new DateTime(1972,11,21),
                                    Sex = true,
                                    Banmedica = "Jorge Alejandro Muñóz Brand"
                                },
                                new UserInitializerVM
                                {
                                    Names = "PABLO MANUEL",
                                    LastF = "PARDO",
                                    LastM = "TAPIA",
                                    RUN = 13232932,
                                    Carnet = 222222222,
                                    Email = "grpiccoli@gmail.com",
                                    Key = "145ppt",
                                    Insurance = InsuranceData.Insurance.Colmena,
                                    Birth = new DateTime(1977,5,5),
                                    Sex = true
                                }
                                //new UserInitializerVM
                                //{
                                //    Name = "CRISTIAN ENRIQUE VALDÉS BARRA",
                                //    RUN = 10818980,
                                //    Carnet = 106366886,
                                //    Email = "grpiccoli@gmail.com",
                                //    Roles = new string[] { "" },
                                //    Key = "test2019",
                                //    Claims = new string[] { "" }
                                //},
                                //new UserInitializerVM
                                //{
                                //    Name = "JAIME ANDRES CLARAMUNT LLULL",
                                //    RUN = 12116504,
                                //    Carnet = 106366886,
                                //    Email = "grpiccoli@gmail.com",
                                //    Roles = new string[] { "" },
                                //    Key = "test2019",
                                //    Claims = new string[] { "" }
                                //},
                    };
                    foreach (var item in users)
                    {
                        var carnet = new Carnet
                        {
                            Id = item.Carnet,
                            NaturalId = item.RUN
                        };
                        await _context.Carnets.AddAsync(carnet).ConfigureAwait(false);
                        //var digitalsignature = new DigitalSignature
                        //{
                        //    Id = item.Carnet,
                        //    NaturalId = item.RUN
                        //};
                        //context.DigitalSignatures.Add(digitalsignature);
                        var patient = new Patient
                        {
                            Insurance = item.Insurance,
                            NaturalId = item.RUN,
                            InsurancePassword = item.Key
                        };
                        if (item.Tramo != 0) patient.Tramo = item.Tramo;
                        await _context.Patients.AddAsync(patient).ConfigureAwait(false);
                        var natural = new Natural
                        {
                            Id = item.RUN,
                            Carnet = carnet,
                            CarnetId = carnet.Id,
                            //DigitalSignatureId = carnet.Id,
                            Patient = patient,
                            Names = item.Names,
                            LastFather = item.LastF,
                            LastMother = item.LastM,
                            FullLastFirst = $"{item.LastF} {item.LastM}, {item.Names}",
                            FullNameFirst = $"{item.Names} {item.LastF} {item.LastM}",
                            Sex = item.Sex,
                            Birth = item.Birth,
                            Discriminator = "Natural",
                            Nationality = "CHILENA"
                        };
                        if (!string.IsNullOrWhiteSpace(item.Banmedica)) natural.BanmedicaName = item.Banmedica;
                        await _context.People.AddAsync(natural).ConfigureAwait(false);
                        var user = new ApplicationUser
                        {
                            UserName = RUT.Format(item.RUN),
                            PhoneNumber = "+56 9 6841 9339",
                            PhoneConfirmationTime = DateTime.Now.AddMinutes(5),
                            PhoneNumberConfirmed = true,
                            Email = item.Email,
                            NormalizedEmail = _normalizer.NormalizeEmail(item.Email),
                            MailConfirmationTime = DateTime.Now.AddMinutes(5),
                            EmailConfirmed = true,
                            LockoutEnabled = false,
                            SecurityStamp = Guid.NewGuid().ToString(),
                            Person = natural
                        };
                        user.NormalizedUserName = _normalizer.NormalizeName(user.UserName);
                        var hasher = new PasswordHasher<ApplicationUser>();
                        var hashedPassword = hasher.HashPassword(user, item.Key);
                        user.PasswordHash = hashedPassword;

                        if (!string.IsNullOrWhiteSpace(item.Claim))
                        {
                            user.Claims.Add(new IdentityUserClaim<string>
                            {
                                ClaimType = item.Claim,
                                ClaimValue = item.Claim
                            });
                        }

                        if (!string.IsNullOrWhiteSpace(item.Role))
                        {
                            var roller = await _context.Roles
                                .SingleOrDefaultAsync(r => r.Name == item.Role)
                                .ConfigureAwait(false);
                            user.UserRoles.Add(new ApplicationUserRole
                            {
                                UserId = user.Id,
                                RoleId = roller.Id
                            });
                        }
                        await _context.Users.AddAsync(user).ConfigureAwait(false);
                    }
                    //context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.DigitalSignatures ON");
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                    //context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.DigitalSignatures OFF");
                    //transaction.Commit();
                }
            }
            return;
        }
    }
}

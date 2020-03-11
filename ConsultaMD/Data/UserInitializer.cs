using ConsultaMD.Extensions;
using ConsultaMD.Models.Entities;
using ConsultaMD.Models.VM;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Data
{
    public class UserInitializer : IUser
    {
        private readonly ApplicationDbContext _context;
        private readonly ILookupNormalizer _normalizer;
        public UserInitializer(
            ApplicationDbContext context,
            ILookupNormalizer normalizer
            )
        {
            _context = context;
            _normalizer = normalizer;
        }
        public async Task Seed()
        {
            using var transaction = _context.Database.BeginTransaction();
            using var roleStore = new RoleStore<ApplicationRole>(_context);
            using var userStore = new UserStore<ApplicationUser>(_context);
            if (!_context.ApplicationUserRoles.Any())
            {
                if (!_context.Users.Any())
                {
                    if (!_context.ApplicationRoles.Any())
                    {
                        var applicationRoles = new List<ApplicationRole> { };
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
                            var roller = _context.Roles.SingleOrDefault(r => r.Name == item.Role);
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
                    transaction.Commit();
                }
            }
            return;
        }
    }
}

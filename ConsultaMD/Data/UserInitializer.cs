using ConsultaMD.Extensions;
using ConsultaMD.Models.Entities;
using ConsultaMD.Models.VM;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Data
{
    public static class UserInitializer
    {
        public static Task Initialize(ApplicationDbContext context, ILookupNormalizer normalizer)
        {
            using (var transaction = context?.Database.BeginTransaction())
            using (var roleStore = new RoleStore<ApplicationRole>(context))
            using (var userStore = new UserStore<ApplicationUser>(context))
                if (context != null)
                {
                    if (!context.ApplicationUserRoles.Any())
                    {
                        if (!context.Users.Any())
                        {
                            if (!context.ApplicationRoles.Any())
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
                                    context.ApplicationRoles.Add(role);
                                }
                                context.SaveChanges();
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
                                context.Carnets.Add(carnet);
                                var digitalsignature = new DigitalSignature
                                {
                                    Id = item.Carnet,
                                    NaturalId = item.RUN
                                };
                                context.DigitalSignatures.Add(digitalsignature);
                                var patient = new Patient
                                {
                                    Insurance = item.Insurance,
                                    NaturalId = item.RUN,
                                    InsurancePassword = item.Key
                                };
                                if (item.Tramo != 0) patient.Tramo = item.Tramo;
                                context.Patients.Add(patient);
                                var natural = new Natural
                                {
                                    Id = item.RUN,
                                    Carnet = carnet,
                                    CarnetId = carnet.Id,
                                    DigitalSignatureId = carnet.Id,
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
                                context.People.Add(natural);
                                var user = new ApplicationUser
                                {
                                    UserName = RUT.Format(item.RUN),
                                    PhoneNumber = "+56968419339",
                                    PhoneConfirmationTime = DateTime.Now.AddMinutes(5),
                                    PhoneNumberConfirmed = true,
                                    Email = item.Email,
                                    NormalizedEmail = normalizer?.Normalize(item.Email),
                                    MailConfirmationTime = DateTime.Now.AddMinutes(5),
                                    EmailConfirmed = true,
                                    LockoutEnabled = false,
                                    SecurityStamp = Guid.NewGuid().ToString(),
                                    Person = natural
                                };
                                user.NormalizedUserName = normalizer?.Normalize(user.UserName);
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
                                    var roller = context.Roles.SingleOrDefault(r => r.Name == item.Role);
                                    user.UserRoles.Add(new ApplicationUserRole
                                    {
                                        UserId = user.Id,
                                        RoleId = roller.Id
                                    });
                                }

                                context.Users.Add(user);
                            }
                            context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.DigitalSignatures ON");
                            context.SaveChanges();
                            context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.DigitalSignatures OFF");
                            transaction.Commit();
                        }
                    }
                }
            return Task.CompletedTask;
        }
    }
}

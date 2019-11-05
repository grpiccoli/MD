﻿using ConsultaMD.Extensions;
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
            if(context != null)
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
                            Name = "GUILLERMO ANTONIO RODRÍGUEZ PICCOLI",
                            RUN = 16124902,
                            Carnet = 519194461,
                            Email = "contacto@epicsolutions.cl",
                            Role = "Administrator",
                            Key = "test2019",
                            Claim = "webmaster"
                        },
                        new UserInitializerVM
                        {
                            Name = "JORGE ALEJANDRO MUNOZ BRAND",
                            RUN = 11927437,
                            Carnet = 111111111,
                            Email = "grpiccoli@gmail.com",
                            Key = "test2019"
                        },
                        new UserInitializerVM
                        {
                            Name = "PABLO MANUEL PARDO TAPIA",
                            RUN = 13232932,
                            Carnet = 222222222,
                            Email = "grpiccoli@gmail.com",
                            Key = "test2019"
                        },
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
                            var carnet = new Carnet { Id = item.Carnet };
                                context.Carnets.Add(carnet);
                                var digitalsignature = new DigitalSignature { Id = item.Carnet };
                                context.DigitalSignatures.Add(digitalsignature);
                                var natural = new Natural
                            {
                                Id = item.RUN,
                                Carnet = carnet,
                                CarnetId = carnet.Id,
                                DigitalSignatureId = carnet.Id,
                                Patient = new Patient
                                {
                                    Insurance = InsuranceData.Insurance.Fonasa
                                },
                                FullNameFirst = item.Name
                            };
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

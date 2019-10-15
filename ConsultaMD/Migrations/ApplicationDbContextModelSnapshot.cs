﻿// <auto-generated />
using System;
using ConsultaMD.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ConsultaMD.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ConsultaMD.Models.Entities.Agenda", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<TimeSpan>("Duration");

                    b.Property<DateTime>("EndTime");

                    b.Property<int>("MediumDoctorId");

                    b.Property<DateTime>("StartTime");

                    b.HasKey("Id");

                    b.HasIndex("MediumDoctorId");

                    b.ToTable("Agenda");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.ApplicationRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Description");

                    b.Property<string>("IPAddress");

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("IsActive");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<DateTime>("MailConfirmationTime");

                    b.Property<DateTime>("MemberSince");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<int>("PersonId");

                    b.Property<DateTime>("PhoneConfirmationTime");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<int>("Rating");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.HasIndex("PersonId");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.AreaCode", b =>
                {
                    b.Property<int>("Id");

                    b.HasKey("Id");

                    b.ToTable("AreaCodes");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.AreaCodeProvince", b =>
                {
                    b.Property<int>("AreaCodeId");

                    b.Property<int>("ProvinceId");

                    b.HasKey("AreaCodeId", "ProvinceId");

                    b.HasIndex("ProvinceId");

                    b.ToTable("AreaCodeProvinces");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.Carnet", b =>
                {
                    b.Property<int>("Id");

                    b.Property<string>("BackImage");

                    b.Property<string>("FrontImage");

                    b.HasKey("Id");

                    b.ToTable("Carnets");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.Census", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Count");

                    b.Property<int?>("LocalityId");

                    b.Property<int>("LocationId");

                    b.Property<DateTime>("Year");

                    b.HasKey("Id");

                    b.HasIndex("LocalityId");

                    b.ToTable("Census");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.CommercialActivity", b =>
                {
                    b.Property<int>("Id");

                    b.Property<int?>("CompanyId");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.ToTable("CommercialActivities");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.Dependency", b =>
                {
                    b.Property<int>("BeneficiaryId");

                    b.Property<int>("DependantId");

                    b.HasKey("BeneficiaryId", "DependantId");

                    b.HasIndex("DependantId");

                    b.ToTable("Dependencies");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.DigitalSignature", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("PathToKey");

                    b.HasKey("Id");

                    b.ToTable("DigitalSignatures");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.Doctor", b =>
                {
                    b.Property<int>("Id");

                    b.Property<DateTime>("Birth");

                    b.Property<int>("DigitalSignatureId");

                    b.Property<string>("Institution");

                    b.Property<string>("Nationality");

                    b.Property<int>("NaturalId");

                    b.Property<DateTime>("RegistryDate");

                    b.Property<bool>("Sex");

                    b.Property<string>("SisId");

                    b.Property<int?>("Specialty");

                    b.Property<string>("Title");

                    b.Property<int>("YearSpecialty");

                    b.Property<int>("YearTitle");

                    b.HasKey("Id");

                    b.HasIndex("DigitalSignatureId");

                    b.HasIndex("NaturalId")
                        .IsUnique();

                    b.ToTable("Doctors");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.InsuranceLocation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Insurance");

                    b.Property<int>("MediumDoctorId");

                    b.Property<string>("Password");

                    b.HasKey("Id");

                    b.HasIndex("MediumDoctorId");

                    b.ToTable("InsuranceLocations");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.Locality", b =>
                {
                    b.Property<int>("Id");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("Name");

                    b.Property<int?>("Surface");

                    b.HasKey("Id");

                    b.ToTable("Localities");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Locality");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.MedicalAttentionMedium", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("MedicalAttentionMediums");

                    b.HasDiscriminator<string>("Discriminator").HasValue("MedicalAttentionMedium");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.MediumDoctor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Color");

                    b.Property<int>("DoctorId");

                    b.Property<int>("MedicalAttentionMediumId");

                    b.Property<bool>("OverTime");

                    b.Property<int>("PriceParticular");

                    b.HasKey("Id");

                    b.HasIndex("DoctorId");

                    b.HasIndex("MedicalAttentionMediumId");

                    b.ToTable("MediumDoctors");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.Patient", b =>
                {
                    b.Property<int>("NaturalId");

                    b.Property<int>("Insurance");

                    b.Property<string>("InsurancePassword");

                    b.HasKey("NaturalId");

                    b.ToTable("Patients");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.Person", b =>
                {
                    b.Property<int>("Id");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("FullLastFirst");

                    b.Property<string>("FullNameFirst");

                    b.HasKey("Id");

                    b.ToTable("People");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Person");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.Place", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("Address");

                    b.Property<string>("CId");

                    b.Property<int>("CommuneId");

                    b.Property<double>("Latitude");

                    b.Property<double>("Longitude");

                    b.Property<string>("Name");

                    b.Property<string>("PhotoId");

                    b.HasKey("Id");

                    b.HasIndex("CommuneId");

                    b.ToTable("Places");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.Polygon", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("LocalityId");

                    b.HasKey("Id");

                    b.HasIndex("LocalityId");

                    b.ToTable("Polygons");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.Publication", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Authorized");

                    b.Property<string>("AuthorizedBy");

                    b.Property<int?>("DoctorId");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("DoctorId");

                    b.ToTable("Publications");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.Subspecialty", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Authorized");

                    b.Property<string>("AuthorizedBy");

                    b.Property<int?>("DoctorId");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("DoctorId");

                    b.ToTable("Subspecialties");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.TimeSlot", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AgendaId");

                    b.Property<DateTime>("EndTime");

                    b.Property<int?>("PatientId");

                    b.Property<DateTime>("StartTime");

                    b.Property<bool>("Taken");

                    b.HasKey("Id");

                    b.HasIndex("AgendaId");

                    b.HasIndex("PatientId");

                    b.ToTable("TimeSlots");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.Vertex", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<double>("Latitude");

                    b.Property<double>("Longitude");

                    b.Property<int>("Order");

                    b.Property<int>("PolygonId");

                    b.HasKey("Id");

                    b.HasIndex("PolygonId");

                    b.ToTable("Vertices");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.HasKey("UserId", "RoleId");

                    b.ToTable("AspNetUserRoles");

                    b.HasDiscriminator<string>("Discriminator").HasValue("IdentityUserRole<string>");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.Commune", b =>
                {
                    b.HasBaseType("ConsultaMD.Models.Entities.Locality");

                    b.Property<int?>("ElectoralDistrict");

                    b.Property<int>("ProvinceId");

                    b.Property<int?>("SenatorialCircunscription");

                    b.HasIndex("ProvinceId");

                    b.HasDiscriminator().HasValue("Commune");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.Province", b =>
                {
                    b.HasBaseType("ConsultaMD.Models.Entities.Locality");

                    b.Property<int>("RegionId");

                    b.HasIndex("RegionId");

                    b.HasDiscriminator().HasValue("Province");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.Region", b =>
                {
                    b.HasBaseType("ConsultaMD.Models.Entities.Locality");

                    b.HasDiscriminator().HasValue("Region");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.HomeVisit", b =>
                {
                    b.HasBaseType("ConsultaMD.Models.Entities.MedicalAttentionMedium");

                    b.Property<int?>("CommuneId");

                    b.HasIndex("CommuneId");

                    b.HasDiscriminator().HasValue("HomeVisit");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.MedicalOffice", b =>
                {
                    b.HasBaseType("ConsultaMD.Models.Entities.MedicalAttentionMedium");

                    b.Property<string>("Appartment");

                    b.Property<string>("Block");

                    b.Property<string>("Floor");

                    b.Property<string>("Office");

                    b.Property<string>("PlaceId");

                    b.HasIndex("PlaceId");

                    b.HasDiscriminator().HasValue("MedicalOffice");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.RemoteAppointment", b =>
                {
                    b.HasBaseType("ConsultaMD.Models.Entities.MedicalAttentionMedium");

                    b.HasDiscriminator().HasValue("RemoteAppointment");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.Company", b =>
                {
                    b.HasBaseType("ConsultaMD.Models.Entities.Person");

                    b.Property<string>("NombreFantasia");

                    b.HasDiscriminator().HasValue("Company");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.Natural", b =>
                {
                    b.HasBaseType("ConsultaMD.Models.Entities.Person");

                    b.Property<int>("CarnetId");

                    b.Property<int>("DoctorId");

                    b.HasIndex("CarnetId");

                    b.HasDiscriminator().HasValue("Natural");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.ApplicationUserRole", b =>
                {
                    b.HasBaseType("Microsoft.AspNetCore.Identity.IdentityUserRole<string>");

                    b.Property<string>("UserId1");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId1");

                    b.HasDiscriminator().HasValue("ApplicationUserRole");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.Agenda", b =>
                {
                    b.HasOne("ConsultaMD.Models.Entities.MediumDoctor", "MediumDoctor")
                        .WithMany("Agendas")
                        .HasForeignKey("MediumDoctorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.ApplicationUser", b =>
                {
                    b.HasOne("ConsultaMD.Models.Entities.Person", "Person")
                        .WithMany()
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.AreaCodeProvince", b =>
                {
                    b.HasOne("ConsultaMD.Models.Entities.AreaCode", "AreaCode")
                        .WithMany("AreaCodeProvinces")
                        .HasForeignKey("AreaCodeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ConsultaMD.Models.Entities.Province", "Province")
                        .WithMany("AreaCodeProvinces")
                        .HasForeignKey("ProvinceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.Census", b =>
                {
                    b.HasOne("ConsultaMD.Models.Entities.Locality", "Locality")
                        .WithMany("Censuses")
                        .HasForeignKey("LocalityId");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.CommercialActivity", b =>
                {
                    b.HasOne("ConsultaMD.Models.Entities.Company")
                        .WithMany("CommercialActivities")
                        .HasForeignKey("CompanyId");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.Dependency", b =>
                {
                    b.HasOne("ConsultaMD.Models.Entities.Patient", "Beneficiary")
                        .WithMany("Dependants")
                        .HasForeignKey("BeneficiaryId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("ConsultaMD.Models.Entities.Natural", "Dependant")
                        .WithMany()
                        .HasForeignKey("DependantId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.Doctor", b =>
                {
                    b.HasOne("ConsultaMD.Models.Entities.DigitalSignature", "DigitalSignature")
                        .WithMany()
                        .HasForeignKey("DigitalSignatureId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ConsultaMD.Models.Entities.Natural", "Natural")
                        .WithOne("Doctor")
                        .HasForeignKey("ConsultaMD.Models.Entities.Doctor", "NaturalId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.InsuranceLocation", b =>
                {
                    b.HasOne("ConsultaMD.Models.Entities.MediumDoctor", "MediumDoctor")
                        .WithMany("InsuranceLocations")
                        .HasForeignKey("MediumDoctorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.MediumDoctor", b =>
                {
                    b.HasOne("ConsultaMD.Models.Entities.Doctor", "Doctor")
                        .WithMany("MediumDoctors")
                        .HasForeignKey("DoctorId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("ConsultaMD.Models.Entities.MedicalAttentionMedium", "MedicalAttentionMedium")
                        .WithMany("MediumDoctors")
                        .HasForeignKey("MedicalAttentionMediumId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.Patient", b =>
                {
                    b.HasOne("ConsultaMD.Models.Entities.Natural", "Natural")
                        .WithOne("Patient")
                        .HasForeignKey("ConsultaMD.Models.Entities.Patient", "NaturalId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.Place", b =>
                {
                    b.HasOne("ConsultaMD.Models.Entities.Commune", "Commune")
                        .WithMany("Places")
                        .HasForeignKey("CommuneId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.Polygon", b =>
                {
                    b.HasOne("ConsultaMD.Models.Entities.Locality", "Locality")
                        .WithMany("Polygons")
                        .HasForeignKey("LocalityId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.Publication", b =>
                {
                    b.HasOne("ConsultaMD.Models.Entities.Doctor", "Doctor")
                        .WithMany("Publications")
                        .HasForeignKey("DoctorId");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.Subspecialty", b =>
                {
                    b.HasOne("ConsultaMD.Models.Entities.Doctor", "Doctor")
                        .WithMany("Subspecialties")
                        .HasForeignKey("DoctorId");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.TimeSlot", b =>
                {
                    b.HasOne("ConsultaMD.Models.Entities.Agenda", "Agenda")
                        .WithMany("TimeSlots")
                        .HasForeignKey("AgendaId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ConsultaMD.Models.Entities.Patient", "Patient")
                        .WithMany("TimeSlotAppointments")
                        .HasForeignKey("PatientId");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.Vertex", b =>
                {
                    b.HasOne("ConsultaMD.Models.Entities.Polygon", "Polygon")
                        .WithMany("Vertices")
                        .HasForeignKey("PolygonId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("ConsultaMD.Models.Entities.ApplicationRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("ConsultaMD.Models.Entities.ApplicationUser")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("ConsultaMD.Models.Entities.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("ConsultaMD.Models.Entities.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.Commune", b =>
                {
                    b.HasOne("ConsultaMD.Models.Entities.Province", "Province")
                        .WithMany("Communes")
                        .HasForeignKey("ProvinceId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.Province", b =>
                {
                    b.HasOne("ConsultaMD.Models.Entities.Region", "Region")
                        .WithMany("Provinces")
                        .HasForeignKey("RegionId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.HomeVisit", b =>
                {
                    b.HasOne("ConsultaMD.Models.Entities.Commune", "Commune")
                        .WithMany("HomeVisits")
                        .HasForeignKey("CommuneId");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.MedicalOffice", b =>
                {
                    b.HasOne("ConsultaMD.Models.Entities.Place", "Place")
                        .WithMany()
                        .HasForeignKey("PlaceId");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.Natural", b =>
                {
                    b.HasOne("ConsultaMD.Models.Entities.Carnet", "Carnet")
                        .WithMany()
                        .HasForeignKey("CarnetId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.ApplicationUserRole", b =>
                {
                    b.HasOne("ConsultaMD.Models.Entities.ApplicationRole", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ConsultaMD.Models.Entities.ApplicationUser")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ConsultaMD.Models.Entities.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId1");
                });
#pragma warning restore 612, 618
        }
    }
}
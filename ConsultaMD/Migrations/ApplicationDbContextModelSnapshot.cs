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

                    b.Property<int>("AgendaEventId");

                    b.Property<DateTime>("EndTime");

                    b.Property<DateTime>("StartTime");

                    b.HasKey("Id");

                    b.HasIndex("AgendaEventId");

                    b.ToTable("Agenda");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.AgendaEvent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<TimeSpan>("Duration");

                    b.Property<DateTime>("EndDateTime");

                    b.Property<int>("Frequency");

                    b.Property<int>("MediumDoctorId");

                    b.Property<DateTime>("StartDateTime");

                    b.HasKey("Id");

                    b.HasIndex("MediumDoctorId");

                    b.ToTable("AgendaEvents");
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

                    b.HasIndex("PersonId")
                        .IsUnique();

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

                    b.Property<int>("NaturalId");

                    b.HasKey("Id");

                    b.HasIndex("NaturalId")
                        .IsUnique();

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

            modelBuilder.Entity("ConsultaMD.Models.Entities.Doctor", b =>
                {
                    b.Property<int>("Id");

                    b.Property<int?>("FonasaLevel");

                    b.Property<int?>("MedicalAttentionId");

                    b.Property<int>("NaturalId");

                    b.Property<DateTime>("RegistryDate");

                    b.Property<string>("SisId");

                    b.Property<int>("YearTitle");

                    b.HasKey("Id");

                    b.HasIndex("NaturalId")
                        .IsUnique();

                    b.ToTable("Doctors");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.DoctorSpecialty", b =>
                {
                    b.Property<int>("DoctorId");

                    b.Property<int>("SpecialtyId");

                    b.HasKey("DoctorId", "SpecialtyId");

                    b.HasIndex("SpecialtyId");

                    b.ToTable("DoctorSpecialties");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.EventDayWeek", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AgendaEventId");

                    b.Property<int>("DayOfWeek");

                    b.HasKey("Id");

                    b.HasIndex("AgendaEventId");

                    b.ToTable("EventDayWeeks");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.InsuranceAgreement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Insurance");

                    b.Property<string>("Password");

                    b.Property<int>("PersonId");

                    b.HasKey("Id");

                    b.HasIndex("PersonId");

                    b.ToTable("InsuranceAgreements");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.InsuranceLocation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Address");

                    b.Property<int>("CommuneId");

                    b.Property<int>("InsuranceAgreementId");

                    b.Property<string>("InsuranceSelector");

                    b.Property<int>("MediumDoctorId");

                    b.Property<string>("PrestacionId");

                    b.HasKey("Id");

                    b.HasIndex("CommuneId");

                    b.HasIndex("InsuranceAgreementId");

                    b.HasIndex("MediumDoctorId");

                    b.HasIndex("PrestacionId");

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

            modelBuilder.Entity("ConsultaMD.Models.Entities.MedicalAttention", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("DoctorId");

                    b.Property<int>("ReservationId");

                    b.Property<DateTime>("StartTime");

                    b.HasKey("Id");

                    b.HasIndex("DoctorId")
                        .IsUnique();

                    b.HasIndex("ReservationId")
                        .IsUnique();

                    b.ToTable("MedicalAttention");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.MedicalAttentionMedium", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("PlaceId");

                    b.HasKey("Id");

                    b.HasIndex("PlaceId");

                    b.ToTable("MedicalAttentionMediums");

                    b.HasDiscriminator<string>("Discriminator").HasValue("MedicalAttentionMedium");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.MedicalCoverage", b =>
                {
                    b.Property<int>("QuoteeId");

                    b.Property<int>("DependantId");

                    b.HasKey("QuoteeId", "DependantId");

                    b.HasIndex("DependantId");

                    b.ToTable("MedicalCoverages");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.MediumDoctor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Color");

                    b.Property<int>("DoctorId");

                    b.Property<int?>("MedicalAttentionMediumId");

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

                    b.Property<int?>("Tramo");

                    b.HasKey("NaturalId");

                    b.ToTable("Patients");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.Payment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.HasKey("Id");

                    b.ToTable("Payment");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.Person", b =>
                {
                    b.Property<int>("Id");

                    b.Property<string>("BanmedicaName");

                    b.Property<string>("Discriminator")
                        .IsRequired();

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

            modelBuilder.Entity("ConsultaMD.Models.Entities.Prestacion", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Copago");

                    b.Property<string>("Description");

                    b.Property<int>("Total");

                    b.HasKey("Id");

                    b.ToTable("Prestacions");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.Reservation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Arrival");

                    b.Property<bool>("Arrived");

                    b.Property<int?>("BondId");

                    b.Property<bool>("Confirmed");

                    b.Property<int>("MedicalAttentionId");

                    b.Property<int>("PatientId");

                    b.Property<int?>("PaymentId");

                    b.Property<int>("TimeSlotId");

                    b.HasKey("Id");

                    b.HasIndex("PatientId");

                    b.HasIndex("PaymentId");

                    b.ToTable("Reservations");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.Specialty", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Specialties");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.TimeSlot", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AgendaId");

                    b.Property<DateTime>("EndTime");

                    b.Property<int?>("ReservationId");

                    b.Property<DateTime>("StartTime");

                    b.HasKey("Id");

                    b.HasIndex("AgendaId");

                    b.HasIndex("ReservationId")
                        .IsUnique()
                        .HasFilter("[ReservationId] IS NOT NULL");

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

                    b.HasDiscriminator().HasValue("HomeVisit");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.MedicalOffice", b =>
                {
                    b.HasBaseType("ConsultaMD.Models.Entities.MedicalAttentionMedium");

                    b.Property<string>("Appartment");

                    b.Property<string>("Block");

                    b.Property<string>("Floor");

                    b.Property<string>("Office");

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

                    b.Property<string>("RazonSocial");

                    b.HasDiscriminator().HasValue("Company");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.Natural", b =>
                {
                    b.HasBaseType("ConsultaMD.Models.Entities.Person");

                    b.Property<int?>("ApplicationUserId");

                    b.Property<DateTime>("Birth");

                    b.Property<int>("CarnetId");

                    b.Property<int>("DoctorId");

                    b.Property<string>("FullLastFirst");

                    b.Property<string>("FullNameFirst");

                    b.Property<string>("LastFather");

                    b.Property<string>("LastMother");

                    b.Property<string>("Names");

                    b.Property<string>("Nationality");

                    b.Property<string>("PassSII");

                    b.Property<bool>("Sex");

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
                    b.HasOne("ConsultaMD.Models.Entities.AgendaEvent", "AgendaEvent")
                        .WithMany("Agendas")
                        .HasForeignKey("AgendaEventId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.AgendaEvent", b =>
                {
                    b.HasOne("ConsultaMD.Models.Entities.MediumDoctor", "MediumDoctor")
                        .WithMany("AgendaEvents")
                        .HasForeignKey("MediumDoctorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.ApplicationUser", b =>
                {
                    b.HasOne("ConsultaMD.Models.Entities.Natural", "Person")
                        .WithOne("ApplicationUser")
                        .HasForeignKey("ConsultaMD.Models.Entities.ApplicationUser", "PersonId")
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

            modelBuilder.Entity("ConsultaMD.Models.Entities.Carnet", b =>
                {
                    b.HasOne("ConsultaMD.Models.Entities.Natural", "Natural")
                        .WithOne("Carnet")
                        .HasForeignKey("ConsultaMD.Models.Entities.Carnet", "NaturalId")
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

            modelBuilder.Entity("ConsultaMD.Models.Entities.Doctor", b =>
                {
                    b.HasOne("ConsultaMD.Models.Entities.Natural", "Natural")
                        .WithOne("Doctor")
                        .HasForeignKey("ConsultaMD.Models.Entities.Doctor", "NaturalId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.DoctorSpecialty", b =>
                {
                    b.HasOne("ConsultaMD.Models.Entities.Doctor", "Doctor")
                        .WithMany("Specialties")
                        .HasForeignKey("DoctorId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ConsultaMD.Models.Entities.Specialty", "Specialty")
                        .WithMany("Doctors")
                        .HasForeignKey("SpecialtyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.EventDayWeek", b =>
                {
                    b.HasOne("ConsultaMD.Models.Entities.AgendaEvent", "AgendaEvent")
                        .WithMany("EventDayWeeks")
                        .HasForeignKey("AgendaEventId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.InsuranceAgreement", b =>
                {
                    b.HasOne("ConsultaMD.Models.Entities.Person", "Person")
                        .WithMany("InsuranceAgreements")
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.InsuranceLocation", b =>
                {
                    b.HasOne("ConsultaMD.Models.Entities.Commune", "Commune")
                        .WithMany("InsuranceLocations")
                        .HasForeignKey("CommuneId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ConsultaMD.Models.Entities.InsuranceAgreement", "InsuranceAgreement")
                        .WithMany("InsuranceLocations")
                        .HasForeignKey("InsuranceAgreementId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ConsultaMD.Models.Entities.MediumDoctor", "MediumDoctor")
                        .WithMany("InsuranceLocations")
                        .HasForeignKey("MediumDoctorId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ConsultaMD.Models.Entities.Prestacion", "Prestacion")
                        .WithMany("InsuranceLocation")
                        .HasForeignKey("PrestacionId");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.MedicalAttention", b =>
                {
                    b.HasOne("ConsultaMD.Models.Entities.Doctor", "Doctor")
                        .WithOne("MedicalAttention")
                        .HasForeignKey("ConsultaMD.Models.Entities.MedicalAttention", "DoctorId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("ConsultaMD.Models.Entities.Reservation", "Reservation")
                        .WithOne("MedicalAttention")
                        .HasForeignKey("ConsultaMD.Models.Entities.MedicalAttention", "ReservationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.MedicalAttentionMedium", b =>
                {
                    b.HasOne("ConsultaMD.Models.Entities.Place", "Place")
                        .WithMany()
                        .HasForeignKey("PlaceId");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.MedicalCoverage", b =>
                {
                    b.HasOne("ConsultaMD.Models.Entities.Natural", "Dependant")
                        .WithMany()
                        .HasForeignKey("DependantId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ConsultaMD.Models.Entities.Patient", "Quotee")
                        .WithMany("MedicalCoverages")
                        .HasForeignKey("QuoteeId")
                        .OnDelete(DeleteBehavior.Restrict);
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

            modelBuilder.Entity("ConsultaMD.Models.Entities.Reservation", b =>
                {
                    b.HasOne("ConsultaMD.Models.Entities.Patient", "Patient")
                        .WithMany("Reservations")
                        .HasForeignKey("PatientId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ConsultaMD.Models.Entities.Payment", "Payment")
                        .WithMany()
                        .HasForeignKey("PaymentId");
                });

            modelBuilder.Entity("ConsultaMD.Models.Entities.TimeSlot", b =>
                {
                    b.HasOne("ConsultaMD.Models.Entities.Agenda", "Agenda")
                        .WithMany("TimeSlots")
                        .HasForeignKey("AgendaId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ConsultaMD.Models.Entities.Reservation", "Reservation")
                        .WithOne("TimeSlot")
                        .HasForeignKey("ConsultaMD.Models.Entities.TimeSlot", "ReservationId")
                        .OnDelete(DeleteBehavior.Restrict);
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

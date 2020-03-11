using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ConsultaMD.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AreaCodes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AreaCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    IPAddress = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Localities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Surface = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    ElectoralDistrict = table.Column<int>(nullable: true),
                    SenatorialCircunscription = table.Column<int>(nullable: true),
                    ProvinceId = table.Column<int>(nullable: true),
                    RegionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Localities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Localities_Localities_ProvinceId",
                        column: x => x.ProvinceId,
                        principalTable: "Localities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Localities_Localities_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Localities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Prestacions",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Total = table.Column<int>(nullable: false),
                    Copago = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prestacions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Specialties",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specialties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AreaCodeProvinces",
                columns: table => new
                {
                    ProvinceId = table.Column<int>(nullable: false),
                    AreaCodeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AreaCodeProvinces", x => new { x.AreaCodeId, x.ProvinceId });
                    table.ForeignKey(
                        name: "FK_AreaCodeProvinces_AreaCodes_AreaCodeId",
                        column: x => x.AreaCodeId,
                        principalTable: "AreaCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AreaCodeProvinces_Localities_ProvinceId",
                        column: x => x.ProvinceId,
                        principalTable: "Localities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Census",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<DateTime>(nullable: false),
                    Count = table.Column<int>(nullable: false),
                    LocationId = table.Column<int>(nullable: false),
                    LocalityId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Census", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Census_Localities_LocalityId",
                        column: x => x.LocalityId,
                        principalTable: "Localities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Places",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Latitude = table.Column<double>(nullable: false),
                    Longitude = table.Column<double>(nullable: false),
                    Address = table.Column<string>(nullable: true),
                    CommuneId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    CId = table.Column<string>(nullable: true),
                    PhotoId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Places", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Places_Localities_CommuneId",
                        column: x => x.CommuneId,
                        principalTable: "Localities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Polygons",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocalityId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Polygons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Polygons_Localities_LocalityId",
                        column: x => x.LocalityId,
                        principalTable: "Localities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MedicalAttentionMediums",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Discriminator = table.Column<string>(nullable: false),
                    PlaceId = table.Column<string>(nullable: true),
                    Block = table.Column<string>(nullable: true),
                    Floor = table.Column<string>(nullable: true),
                    Appartment = table.Column<string>(nullable: true),
                    Office = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalAttentionMediums", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicalAttentionMediums_Places_PlaceId",
                        column: x => x.PlaceId,
                        principalTable: "Places",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Vertices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Latitude = table.Column<double>(nullable: false),
                    Longitude = table.Column<double>(nullable: false),
                    PolygonId = table.Column<int>(nullable: false),
                    Order = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vertices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vertices_Polygons_PolygonId",
                        column: x => x.PolygonId,
                        principalTable: "Polygons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimeSlots",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AgendaId = table.Column<int>(nullable: false),
                    ReservationId = table.Column<int>(nullable: true),
                    StartTime = table.Column<DateTime>(nullable: false),
                    EndTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeSlots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Agenda",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AgendaEventId = table.Column<int>(nullable: false),
                    StartTime = table.Column<DateTime>(nullable: false),
                    EndTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agenda", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventDayWeeks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AgendaEventId = table.Column<int>(nullable: false),
                    DayOfWeek = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventDayWeeks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    UserId1 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "People",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    BanmedicaName = table.Column<string>(nullable: true),
                    RazonSocial = table.Column<string>(nullable: true),
                    NombreFantasia = table.Column<string>(nullable: true),
                    CarnetId = table.Column<int>(nullable: true),
                    DoctorId = table.Column<int>(nullable: true),
                    PassSII = table.Column<string>(nullable: true),
                    LastFather = table.Column<string>(nullable: true),
                    LastMother = table.Column<string>(nullable: true),
                    Names = table.Column<string>(nullable: true),
                    FullNameFirst = table.Column<string>(nullable: true),
                    FullLastFirst = table.Column<string>(nullable: true),
                    Sex = table.Column<bool>(nullable: true),
                    Birth = table.Column<DateTime>(nullable: true),
                    Nationality = table.Column<string>(nullable: true),
                    ApplicationUserId = table.Column<int>(nullable: true),
                    CustomerId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    Rating = table.Column<int>(nullable: false),
                    MemberSince = table.Column<DateTime>(nullable: false),
                    PhoneConfirmationTime = table.Column<DateTime>(nullable: false),
                    MailConfirmationTime = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    PersonId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Carnets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    NaturalId = table.Column<int>(nullable: false),
                    BackImage = table.Column<string>(nullable: true),
                    FrontImage = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carnets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Carnets_People_NaturalId",
                        column: x => x.NaturalId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommercialActivities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    CompanyId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommercialActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommercialActivities_People_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerId = table.Column<string>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    PayMode = table.Column<int>(nullable: false),
                    CreditCardType = table.Column<string>(nullable: true),
                    Last4CardDigits = table.Column<string>(nullable: true),
                    ExternalId = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    RegisterDate = table.Column<DateTime>(nullable: false),
                    Token = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustomerId);
                    table.ForeignKey(
                        name: "FK_Customers_People_ExternalId",
                        column: x => x.ExternalId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Doctors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    NaturalId = table.Column<int>(nullable: false),
                    RegistryDate = table.Column<DateTime>(nullable: false),
                    SisId = table.Column<string>(nullable: true),
                    YearTitle = table.Column<int>(nullable: false),
                    FonasaLevel = table.Column<int>(nullable: true),
                    MedicalAttentionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doctors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Doctors_People_NaturalId",
                        column: x => x.NaturalId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InsuranceAgreements",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Insurance = table.Column<int>(nullable: false),
                    PersonId = table.Column<int>(nullable: false),
                    Password = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceAgreements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InsuranceAgreements_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    NaturalId = table.Column<int>(nullable: false),
                    Insurance = table.Column<int>(nullable: false),
                    InsurancePassword = table.Column<string>(nullable: true),
                    Tramo = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.NaturalId);
                    table.ForeignKey(
                        name: "FK_Patients_People_NaturalId",
                        column: x => x.NaturalId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DoctorSpecialties",
                columns: table => new
                {
                    DoctorId = table.Column<int>(nullable: false),
                    SpecialtyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorSpecialties", x => new { x.DoctorId, x.SpecialtyId });
                    table.ForeignKey(
                        name: "FK_DoctorSpecialties_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DoctorSpecialties_Specialties_SpecialtyId",
                        column: x => x.SpecialtyId,
                        principalTable: "Specialties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MediumDoctors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DoctorId = table.Column<int>(nullable: false),
                    MedicalAttentionMediumId = table.Column<int>(nullable: true),
                    PriceParticular = table.Column<int>(nullable: false),
                    OverTime = table.Column<bool>(nullable: false),
                    Color = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediumDoctors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediumDoctors_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MediumDoctors_MedicalAttentionMediums_MedicalAttentionMediumId",
                        column: x => x.MedicalAttentionMediumId,
                        principalTable: "MedicalAttentionMediums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MedicalCoverages",
                columns: table => new
                {
                    QuoteeId = table.Column<int>(nullable: false),
                    DependantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalCoverages", x => new { x.QuoteeId, x.DependantId });
                    table.ForeignKey(
                        name: "FK_MedicalCoverages_People_DependantId",
                        column: x => x.DependantId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MedicalCoverages_Patients_QuoteeId",
                        column: x => x.QuoteeId,
                        principalTable: "Patients",
                        principalColumn: "NaturalId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(nullable: false),
                    TimeSlotId = table.Column<int>(nullable: false),
                    PaymentId = table.Column<int>(nullable: true),
                    BondId = table.Column<int>(nullable: true),
                    Confirmed = table.Column<bool>(nullable: false),
                    Arrived = table.Column<bool>(nullable: false),
                    Arrival = table.Column<DateTime>(nullable: false),
                    MedicalAttentionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservations_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "NaturalId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservations_Payment_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AgendaEvents",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MediumDoctorId = table.Column<int>(nullable: false),
                    StartDateTime = table.Column<DateTime>(nullable: false),
                    EndDateTime = table.Column<DateTime>(nullable: false),
                    Duration = table.Column<TimeSpan>(nullable: false),
                    Frequency = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgendaEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgendaEvents_MediumDoctors_MediumDoctorId",
                        column: x => x.MediumDoctorId,
                        principalTable: "MediumDoctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InsuranceLocations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MediumDoctorId = table.Column<int>(nullable: false),
                    InsuranceAgreementId = table.Column<int>(nullable: false),
                    InsuranceSelector = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    CommuneId = table.Column<int>(nullable: false),
                    PrestacionId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InsuranceLocations_Localities_CommuneId",
                        column: x => x.CommuneId,
                        principalTable: "Localities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InsuranceLocations_InsuranceAgreements_InsuranceAgreementId",
                        column: x => x.InsuranceAgreementId,
                        principalTable: "InsuranceAgreements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InsuranceLocations_MediumDoctors_MediumDoctorId",
                        column: x => x.MediumDoctorId,
                        principalTable: "MediumDoctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InsuranceLocations_Prestacions_PrestacionId",
                        column: x => x.PrestacionId,
                        principalTable: "Prestacions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MedicalAttention",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartTime = table.Column<DateTime>(nullable: false),
                    ReservationId = table.Column<int>(nullable: false),
                    DoctorId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalAttention", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicalAttention_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MedicalAttention_Reservations_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "Reservations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agenda_AgendaEventId",
                table: "Agenda",
                column: "AgendaEventId");

            migrationBuilder.CreateIndex(
                name: "IX_AgendaEvents_MediumDoctorId",
                table: "AgendaEvents",
                column: "MediumDoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_AreaCodeProvinces_ProvinceId",
                table: "AreaCodeProvinces",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_UserId1",
                table: "AspNetUserRoles",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PersonId",
                table: "AspNetUsers",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Carnets_NaturalId",
                table: "Carnets",
                column: "NaturalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Census_LocalityId",
                table: "Census",
                column: "LocalityId");

            migrationBuilder.CreateIndex(
                name: "IX_CommercialActivities_CompanyId",
                table: "CommercialActivities",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_ExternalId",
                table: "Customers",
                column: "ExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_NaturalId",
                table: "Doctors",
                column: "NaturalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DoctorSpecialties_SpecialtyId",
                table: "DoctorSpecialties",
                column: "SpecialtyId");

            migrationBuilder.CreateIndex(
                name: "IX_EventDayWeeks_AgendaEventId",
                table: "EventDayWeeks",
                column: "AgendaEventId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceAgreements_PersonId",
                table: "InsuranceAgreements",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceLocations_CommuneId",
                table: "InsuranceLocations",
                column: "CommuneId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceLocations_InsuranceAgreementId",
                table: "InsuranceLocations",
                column: "InsuranceAgreementId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceLocations_MediumDoctorId",
                table: "InsuranceLocations",
                column: "MediumDoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceLocations_PrestacionId",
                table: "InsuranceLocations",
                column: "PrestacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Localities_ProvinceId",
                table: "Localities",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_Localities_RegionId",
                table: "Localities",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalAttention_DoctorId",
                table: "MedicalAttention",
                column: "DoctorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedicalAttention_ReservationId",
                table: "MedicalAttention",
                column: "ReservationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedicalAttentionMediums_PlaceId",
                table: "MedicalAttentionMediums",
                column: "PlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalCoverages_DependantId",
                table: "MedicalCoverages",
                column: "DependantId");

            migrationBuilder.CreateIndex(
                name: "IX_MediumDoctors_DoctorId",
                table: "MediumDoctors",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_MediumDoctors_MedicalAttentionMediumId",
                table: "MediumDoctors",
                column: "MedicalAttentionMediumId");

            migrationBuilder.CreateIndex(
                name: "IX_People_CustomerId",
                table: "People",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Places_CommuneId",
                table: "Places",
                column: "CommuneId");

            migrationBuilder.CreateIndex(
                name: "IX_Polygons_LocalityId",
                table: "Polygons",
                column: "LocalityId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_PatientId",
                table: "Reservations",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_PaymentId",
                table: "Reservations",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeSlots_AgendaId",
                table: "TimeSlots",
                column: "AgendaId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeSlots_ReservationId",
                table: "TimeSlots",
                column: "ReservationId",
                unique: true,
                filter: "[ReservationId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Vertices_PolygonId",
                table: "Vertices",
                column: "PolygonId");

            migrationBuilder.AddForeignKey(
                name: "FK_TimeSlots_Reservations_ReservationId",
                table: "TimeSlots",
                column: "ReservationId",
                principalTable: "Reservations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TimeSlots_Agenda_AgendaId",
                table: "TimeSlots",
                column: "AgendaId",
                principalTable: "Agenda",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Agenda_AgendaEvents_AgendaEventId",
                table: "Agenda",
                column: "AgendaEventId",
                principalTable: "AgendaEvents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventDayWeeks_AgendaEvents_AgendaEventId",
                table: "EventDayWeeks",
                column: "AgendaEventId",
                principalTable: "AgendaEvents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId1",
                table: "AspNetUserRoles",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_People_Customers_CustomerId",
                table: "People",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_People_ExternalId",
                table: "Customers");

            migrationBuilder.DropTable(
                name: "AreaCodeProvinces");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Carnets");

            migrationBuilder.DropTable(
                name: "Census");

            migrationBuilder.DropTable(
                name: "CommercialActivities");

            migrationBuilder.DropTable(
                name: "DoctorSpecialties");

            migrationBuilder.DropTable(
                name: "EventDayWeeks");

            migrationBuilder.DropTable(
                name: "InsuranceLocations");

            migrationBuilder.DropTable(
                name: "MedicalAttention");

            migrationBuilder.DropTable(
                name: "MedicalCoverages");

            migrationBuilder.DropTable(
                name: "TimeSlots");

            migrationBuilder.DropTable(
                name: "Vertices");

            migrationBuilder.DropTable(
                name: "AreaCodes");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Specialties");

            migrationBuilder.DropTable(
                name: "InsuranceAgreements");

            migrationBuilder.DropTable(
                name: "Prestacions");

            migrationBuilder.DropTable(
                name: "Agenda");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "Polygons");

            migrationBuilder.DropTable(
                name: "AgendaEvents");

            migrationBuilder.DropTable(
                name: "Patients");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "MediumDoctors");

            migrationBuilder.DropTable(
                name: "Doctors");

            migrationBuilder.DropTable(
                name: "MedicalAttentionMediums");

            migrationBuilder.DropTable(
                name: "Places");

            migrationBuilder.DropTable(
                name: "Localities");

            migrationBuilder.DropTable(
                name: "People");

            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}

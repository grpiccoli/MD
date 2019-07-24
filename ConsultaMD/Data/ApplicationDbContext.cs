using ConsultaMD.Models;
using ConsultaMD.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ConsultaMD.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.Entity<AppRole>()
                .HasMany(e => e.Users)
                .WithOne()
                .HasForeignKey(e => e.RoleId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AppUser>()
                .HasMany(e => e.Claims)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AppUser>()
                .HasMany(e => e.Roles)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AreaCodeProvincia>()
                .HasKey(p => new { p.AreaCodeId, p.ProvinciaId });

            builder.Entity<DoctorMedicalAttention>()
                .HasKey(p => new { p.DoctorId, p.MedicalAttentionId });
        }
        public DbSet<IdentityUserClaim<string>> IdentityUserClaim { get; set; }
        public DbSet<IdentityUserRole<string>> IdentityUserRole { get; set; }
        public DbSet<Address> Address { get; set; }
        public DbSet<Agenda> Agenda { get; set; }
        public DbSet<Appointment> Appointment { get; set; }
        public DbSet<AppRole> AppRole { get; set; }
        public DbSet<AppUser> AppUser { get; set; }
        public DbSet<AppUserRole> AppUserRole { get; set; }
        public DbSet<AreaCode> AreaCode { get; set; }
        public DbSet<AreaCodeProvincia> AreaCodeProvincia { get; set; }
        public DbSet<Carnet> Carnet { get; set; }
        public DbSet<Census> Census { get; set; }
        public DbSet<CommercialActivity> CommercialActivity { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<Comuna> Comuna { get; set; }
        public DbSet<Coordinate> Coordinate { get; set; }
        public DbSet<CreditCard> CreditCard { get; set; }
        public DbSet<DigitalSignature> DigitalSignature { get; set; }
        public DbSet<Doctor> Doctor { get; set; }
        public DbSet<DoctorMedicalAttention> DoctorMedicalAttention { get; set; }
        public DbSet<HomeVisit> HomeVisit { get; set; }
        public DbSet<InsuranceCompany> InsuranceCompany { get; set; }
        public DbSet<InsuranceLocation> InsuranceLocation { get; set; }
        public DbSet<MedicalAttention> MedicalAttention { get; set; }
        public DbSet<MedicalOffice> MedicalOffice { get; set; }
        public DbSet<Patient> Patient { get; set; }
        public DbSet<Polygon> Polygon { get; set; }
        public DbSet<Provincia> Provincia { get; set; }
        public DbSet<Region> Region { get; set; }
        public DbSet<RemoteAppointment> RemoteAppointment { get; set; }
    }
}

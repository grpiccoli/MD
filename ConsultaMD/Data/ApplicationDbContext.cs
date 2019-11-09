﻿using ConsultaMD.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ConsultaMD.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if(modelBuilder != null)
            {
                base.OnModelCreating(modelBuilder);
                // Customize the ASP.NET Identity model and override the defaults if needed.
                // For example, you can rename the ASP.NET Identity table names and more.
                // Add your customizations after calling base.OnModelCreating(builder);

                modelBuilder.Entity<ApplicationUser>(b =>
                {
                    // Each User can have many UserClaims
                    b.HasMany(e => e.Claims)
                        .WithOne()
                        .HasForeignKey(uc => uc.UserId)
                        .IsRequired();
                    b.HasMany(e => e.UserRoles)
                    .WithOne()
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
                });

                modelBuilder.Entity<ApplicationRole>(b =>
                {
                    // Each Role can have many entries in the UserRole join table
                    b.HasMany(e => e.UserRoles)
                        .WithOne(e => e.Role)
                        .HasForeignKey(ur => ur.RoleId)
                        .IsRequired();
                });

                modelBuilder.Entity<AreaCodeProvince>(a => {
                    a.HasKey(p => new { p.AreaCodeId, p.ProvinceId });

                    a.HasOne(md => md.AreaCode)
                        .WithMany(d => d.AreaCodeProvinces)
                        .HasForeignKey(md => md.AreaCodeId);

                    a.HasOne(md => md.Province)
                        .WithMany(d => d.AreaCodeProvinces)
                        .HasForeignKey(md => md.ProvinceId);
                });

                modelBuilder.Entity<Commune>()
                    .HasOne(p => p.Province)
                    .WithMany(p => p.Communes)
                    .HasForeignKey(i => i.ProvinceId)
                    .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<DoctorSpecialty>(a =>
                {
                    a.HasKey(p => new { p.DoctorId, p.SpecialtyId });

                    a.HasOne(md => md.Doctor)
                        .WithMany(d => d.Specialties)
                        .HasForeignKey(md => md.DoctorId);

                    a.HasOne(md => md.Specialty)
                        .WithMany(d => d.Doctors)
                        .HasForeignKey(md => md.SpecialtyId);
                });

                modelBuilder.Entity<Province>(r =>
                {
                    r.HasOne(p => p.Region)
                    .WithMany(p => p.Provinces)
                    .HasForeignKey(i => i.RegionId)
                    .OnDelete(DeleteBehavior.Restrict);
                    r.HasMany(p => p.Communes)
                    .WithOne(c => c.Province)
                    .HasForeignKey(c => c.ProvinceId)
                    .OnDelete(DeleteBehavior.Restrict);
                });

                modelBuilder.Entity<Region>()
                    .HasMany(r => r.Provinces)
                    .WithOne(p => p.Region)
                    .HasForeignKey(r => r.RegionId)
                    .OnDelete(DeleteBehavior.Restrict);

                //modelBuilder.Entity<Locality>()
                //    .HasOne(l => l.ParentLocality)
                //    .WithMany(p => p.ChildLocalities)
                //    .HasForeignKey(p => p.ParentLocalityId)
                //    .IsRequired(false)
                //    .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<Place>()
                    .HasOne(p => p.Commune)
                    .WithMany(c => c.Places)
                    .HasForeignKey(i => i.CommuneId)
                    .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<MediumDoctor>(entity =>
                {
                    //entity.HasKey(p => new { p.DoctorId, p.MedicalAttentionMediumId });
                    entity.HasOne(md => md.Doctor)
                    .WithMany(d => d.MediumDoctors)
                    .HasForeignKey(md => md.DoctorId)
                    .OnDelete(DeleteBehavior.Restrict);

                    entity.HasOne(md => md.MedicalAttentionMedium)
                    .WithMany(d => d.MediumDoctors)
                    .HasForeignKey(md => md.MedicalAttentionMediumId)
                    .OnDelete(DeleteBehavior.Restrict);
                });

                modelBuilder.Entity<Patient>()
                    .HasKey(p => new { p.NaturalId });

                modelBuilder.Entity<Natural>(entity => {
                    entity.HasOne(d => d.Doctor)
                    .WithOne(n => n.Natural)
                    .HasForeignKey<Doctor>(n => n.NaturalId);
                    entity.HasOne(n => n.DigitalSignature)
                    .WithOne(d => d.Natural)
                    .HasForeignKey<DigitalSignature>(n => n.NaturalId)
                    .OnDelete(DeleteBehavior.Restrict);
                });

                modelBuilder.Entity<DigitalSignature>()
                    .HasOne(d => d.Natural)
                    .WithOne(d => d.DigitalSignature)
                    .HasForeignKey<Natural>(d => d.DigitalSignatureId);

                modelBuilder.Entity<TimeSlot>()
                    .HasOne(r => r.Reservation)
                    .WithOne(t => t.TimeSlot)
                    .HasForeignKey<Reservation>(t => t.TimeSlotId);

                modelBuilder.Entity<Reservation>(res => {
                    res.HasOne(r => r.Patient)
                    .WithMany(p => p.Reservations)
                    .HasForeignKey(r => r.PatientId);
                    res.HasOne(r => r.TimeSlot)
                    .WithOne(t => t.Reservation)
                    .HasForeignKey<TimeSlot>(t => t.ReservationId);
                });

                modelBuilder.Entity<MedicalCoverage>(d => {
                    d.HasKey(p => new { p.QuoteeId, p.DependantId });
                    d.HasOne(md => md.Quotee)
                        .WithMany(p => p.MedicalCoverages)
                        .HasForeignKey(md => md.QuoteeId);
                    d.HasOne(p => p.Quotee)
                        .WithMany(c => c.MedicalCoverages)
                        .HasForeignKey(i => i.QuoteeId)
                        .OnDelete(DeleteBehavior.Restrict);
                });
            }
        }
        public DbSet<IdentityUserClaim<string>> IdentityUserClaims { get; set; }
        public DbSet<IdentityUserRole<string>> IdentityUserRoles { get; set; }
        public DbSet<ApplicationRole> ApplicationRoles { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ApplicationUserRole> ApplicationUserRoles { get; set; }
        public DbSet<Agenda> Agenda { get; set; }
        public DbSet<AreaCode> AreaCodes { get; set; }
        public DbSet<AreaCodeProvince> AreaCodeProvinces { get; set; }
        public DbSet<Carnet> Carnets { get; set; }
        public DbSet<Census> Census { get; set; }
        public DbSet<CommercialActivity> CommercialActivities { get; set; }
        public DbSet<Commune> Communes { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<DigitalSignature> DigitalSignatures { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<DoctorSpecialty> DoctorSpecialties { get; set; }
        public DbSet<HomeVisit> HomeVisits { get; set; }
        public DbSet<InsuranceLocation> InsuranceLocations { get; set; }
        public DbSet<MedicalCoverage> MedicalCoverages { get; set; }
        public DbSet<MedicalOffice> MedicalOffices { get; set; }
        public DbSet<MediumDoctor> MediumDoctors { get; set; }
        public DbSet<Natural> Naturals { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Place> Places { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<Polygon> Polygons { get; set; }
        //public DbSet<Publication> Publications { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<RemoteAppointment> RemoteAppointments { get; set; }
        //public DbSet<Subspecialty> Subspecialties { get; set; }
        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<TimeSlot> TimeSlots { get; set; }
        public DbSet<Vertex> Vertices { get; set; }
        public DbSet<Locality> Localities { get; set; }
        public DbSet<MedicalAttentionMedium> MedicalAttentionMediums { get; set; }
    }
}

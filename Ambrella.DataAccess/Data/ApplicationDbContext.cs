using Ambrella.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Ambrella.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Building> Buildings { get; set; }
        public DbSet<Contract> Contracts { get; set; }

        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanyInspectionReport> CompanyInspectionReports { get; set; }
        public DbSet<BuildingInspectionReport> BuildingInspectionReports { get; set; }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ApplicationBuilding> ApplicationBuildings { get; set; }
        public DbSet<StudentApplicationBuilding> StudentApplicationBuildings { get; set; }
        public DbSet<ApplicationInstitution> ApplicationInstitutions { get; set; }
        public DbSet<SendReportCompanyInspector> SendReportCompanyInspectors { get; set; }
        public DbSet<SendReportBuildingInspector> SendReportBuildingInspectors { get; set; }

        public DbSet<ApplicationCompany> ApplicationCompanies { get; set; }
        public DbSet<InspectorAssign> InspectorAssigns { get; set; }
        public DbSet<InstitutionAssign> InstitutionAssigns { get; set; }

        
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<StudentNotification> StudentNotifications { get; set; }

        public DbSet<AssignmentNotification> AssignmentNotifications { get; set; }
        public DbSet<InstitutionAssignNotification> InstitutionAssignNotifications { get; set; }

        public DbSet<InspectorRecieveNotification> InspectorRecieveNotifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
         

            var enumConverter = new EnumToStringConverter<ApplicationStatus>();
            modelBuilder.Entity<ApplicationBuilding>()
                .Property(e => e.Status)
                .HasConversion(enumConverter);

            var enumConverterr = new EnumToStringConverter<ApplicationStatuss>();
            modelBuilder.Entity<ApplicationCompany>()
                .Property(e => e.Status)
                .HasConversion(enumConverterr);
            var enumConvert = new EnumToStringConverter<AssignmentStatus>();
            modelBuilder.Entity<InspectorAssign>()
                .Property(e => e.Status)
                .HasConversion(enumConvert);
            var enumConver = new EnumToStringConverter<SendReportStatus>();
            modelBuilder.Entity<SendReportCompanyInspector>()
                .Property(e => e.Status)
                .HasConversion(enumConver);
            var enumConve = new EnumToStringConverter<StudentStatus>();
            modelBuilder.Entity<StudentApplicationBuilding>()
                .Property(e => e.Status)
                .HasConversion(enumConve);
            var enumConv = new EnumToStringConverter<InstitutionAssignmentStatus>();
            modelBuilder.Entity<InstitutionAssign>()
                .Property(e => e.Status)
                .HasConversion(enumConv);
            var enumCon = new EnumToStringConverter<BuildingReportStatus>();
            modelBuilder.Entity<SendReportBuildingInspector>()
                .Property(e => e.Status)
                .HasConversion(enumCon);
            var enumCo = new EnumToStringConverter<ContractStatus>();
            modelBuilder.Entity<Contract>()
                .Property(e => e.Status)
                .HasConversion(enumCo);





            modelBuilder.Entity<ApplicationBuilding>()
    .HasOne(ab => ab.Building)
    .WithMany()
    .HasForeignKey(ab => ab.BuildingId)
    .OnDelete(DeleteBehavior.Restrict); // or DeleteBehavior.NoAction
            modelBuilder.Entity<ApplicationCompany>()
   .HasOne(ab => ab.Company)
   .WithMany()
   .HasForeignKey(ab => ab.CompanyId)
   .OnDelete(DeleteBehavior.Restrict); // or DeleteBehavior.NoAction
            modelBuilder.Entity<InspectorAssign>()
   .HasOne(ab => ab.Company)
   .WithMany()
   .HasForeignKey(ab => ab.CompanyId)
   .OnDelete(DeleteBehavior.Restrict); // or DeleteBehavior.NoAction
            modelBuilder.Entity<InstitutionAssign>()
     .HasOne(ab => ab.Building)
     .WithMany()
     .HasForeignKey(ab => ab.BuildingId) // Replace 'BuildingId' with the actual property name
     .OnDelete(DeleteBehavior.Restrict); // or DeleteBehavior.NoAction
            modelBuilder.Entity<SendReportCompanyInspector>()
   .HasOne(ab => ab.CompanyInspectionReport)
   .WithMany()
   .HasForeignKey(ab => ab.CompanyInspectionReportId)
   .OnDelete(DeleteBehavior.Restrict); // or DeleteBehavior.NoAction
            modelBuilder.Entity<SendReportBuildingInspector>()
  .HasOne(ab => ab.BuildingInspectionReport)
  .WithMany()
  .HasForeignKey(ab => ab.BuildingInspectionReportId)
  .OnDelete(DeleteBehavior.Restrict); // or DeleteBehavior.NoAction



            modelBuilder.Entity<ApplicationBuilding>()
    .HasOne(ab => ab.Landlord)
    .WithMany()
    .HasForeignKey(ab => ab.LandlordId)
    .OnDelete(DeleteBehavior.Restrict); // or DeleteBehavior.NoAction
            modelBuilder.Entity<ApplicationCompany>()
   .HasOne(ab => ab.Landlord)
   .WithMany()
   .HasForeignKey(ab => ab.LandlordId)
   .OnDelete(DeleteBehavior.Restrict); // or DeleteBehavior.NoAction
            modelBuilder.Entity<SendReportCompanyInspector>()
   .HasOne(ab => ab.Inspector)
   .WithMany()
   .HasForeignKey(ab => ab.InspectorId)
   .OnDelete(DeleteBehavior.Restrict); // or DeleteBehavior.NoAction
            modelBuilder.Entity<SendReportBuildingInspector>()
  .HasOne(ab => ab.Inspector)
  .WithMany()
  .HasForeignKey(ab => ab.InspectorId)
  .OnDelete(DeleteBehavior.Restrict); // or DeleteBehavior.NoAction
            modelBuilder.Entity<StudentApplicationBuilding>()
           .HasOne(ab => ab.Student)
           .WithMany()
           .HasForeignKey(ab => ab.StudentId)
           .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudentApplicationBuilding>()
                .HasOne(ab => ab.Landlord)
                .WithMany()
                .HasForeignKey(ab => ab.LandlordId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<ApplicationInstitution>()
                .HasOne(ai => ai.Student)
                .WithMany()
                .HasForeignKey(ai => ai.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ApplicationInstitution>()
                .HasOne(ai => ai.Institution)
                .WithMany()
                .HasForeignKey(ai => ai.InstitutionId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<ApplicationBuilding>()
        .HasOne(ab => ab.Institution)
        .WithMany()
        .HasForeignKey(ab => ab.InstitutionId)
        .OnDelete(DeleteBehavior.Restrict); // or the appropriate delete behavior
            modelBuilder.Entity<ApplicationCompany>()
        .HasOne(ab => ab.Admin)
        .WithMany()
        .HasForeignKey(ab => ab.AdminId)
        .OnDelete(DeleteBehavior.Restrict); // or the appropriate delete behavior
            modelBuilder.Entity<SendReportCompanyInspector>()
       .HasOne(ab => ab.Admin)
       .WithMany()
       .HasForeignKey(ab => ab.AdminId)
       .OnDelete(DeleteBehavior.Restrict); // or the appropriate delete behavior
            modelBuilder.Entity<SendReportBuildingInspector>()
      .HasOne(ab => ab.Institution)
      .WithMany()
      .HasForeignKey(ab => ab.InstitutionId)
      .OnDelete(DeleteBehavior.Restrict); // or the appropriate delete behavior
            modelBuilder.Entity<SendReportCompanyInspector>()
      .HasOne(ab => ab.Landlord)
      .WithMany()
      .HasForeignKey(ab => ab.LandlordId)
      .OnDelete(DeleteBehavior.Restrict); // or the appropriate delete behavior
            modelBuilder.Entity<SendReportBuildingInspector>()
     .HasOne(ab => ab.Landlord)
     .WithMany()
     .HasForeignKey(ab => ab.LandlordId)
     .OnDelete(DeleteBehavior.Restrict); // or the appropriate delete behavior

            modelBuilder.Entity<Contract>()
            .HasOne(c => c.Institution)
            .WithMany()
            .HasForeignKey(c => c.InstitutionId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Contract>()
                .HasOne(c => c.Landlord)
                .WithMany()
                .HasForeignKey(c => c.LandlordId)
                .OnDelete(DeleteBehavior.Restrict);



            modelBuilder.Entity<InspectorAssign>()
       .HasOne(ab => ab.Inspector)
       .WithMany()
       .HasForeignKey(ab => ab.InspectorId)
       .OnDelete(DeleteBehavior.Restrict); // or the appropriate delete behavior
            modelBuilder.Entity<InstitutionAssign>()
     .HasOne(ab => ab.Inspector)
     .WithMany()
     .HasForeignKey(ab => ab.InspectorId)
     .OnDelete(DeleteBehavior.Restrict); // or the appropriate delete behavior

            modelBuilder.Entity<Notification>()
      .HasOne(ab => ab.Landlord)
      .WithMany()
      .HasForeignKey(ab => ab.LandlordId)
      .OnDelete(DeleteBehavior.Restrict); // or the appropriate delete behavior
            modelBuilder.Entity<InspectorRecieveNotification>()
     .HasOne(ab => ab.Inspector)
     .WithMany()
     .HasForeignKey(ab => ab.InspectorId)
     .OnDelete(DeleteBehavior.Restrict); // or the appropriate delete behavior
            modelBuilder.Entity<StudentNotification>()
   .HasOne(ab => ab.Student)
   .WithMany()
   .HasForeignKey(ab => ab.StudentId)
   .OnDelete(DeleteBehavior.Restrict); // or the appropriate delete behavior

        }



    }

}


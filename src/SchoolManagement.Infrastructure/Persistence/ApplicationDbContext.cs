using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Student> Students => Set<Student>();
    public DbSet<User> Users => Set<User>();
    public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<PeriodAttendanceRecord> PeriodAttendanceRecords => Set<PeriodAttendanceRecord>();
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<TeacherClassSubjectAllocation> TeacherClassSubjectAllocations => Set<TeacherClassSubjectAllocation>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<CourseSection> CourseSections => Set<CourseSection>();
    public DbSet<CourseSubject> CourseSubjects => Set<CourseSubject>();
    public DbSet<TimetableEntry> TimetableEntries => Set<TimetableEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(x => x.Username).IsUnique();
            entity.Property(x => x.Username).HasMaxLength(100).IsRequired();
            entity.Property(x => x.PasswordHash).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Role).HasMaxLength(50).IsRequired();
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Class).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Section).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(256);
            entity.Property(x => x.Phone).HasMaxLength(30);
            entity.Property(x => x.AddressLine1).HasMaxLength(200);
            entity.Property(x => x.AddressLine2).HasMaxLength(200);
            entity.Property(x => x.City).HasMaxLength(100);
            entity.Property(x => x.State).HasMaxLength(100);
            entity.Property(x => x.PostalCode).HasMaxLength(20);
            entity.Property(x => x.ParentGuardianName).HasMaxLength(200);
            entity.Property(x => x.ParentGuardianPhone).HasMaxLength(30);
            entity.Property(x => x.ParentGuardianEmail).HasMaxLength(256);
            entity.Property(x => x.AdmissionNumber).HasMaxLength(50);
            entity.Property(x => x.EmergencyContact).HasMaxLength(200);
            entity.Property(x => x.Notes).HasMaxLength(2000);
            entity.Property(x => x.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<AttendanceRecord>(entity =>
        {
            entity.ToTable("AttendanceRecords");
            entity.Property(x => x.Class).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Section).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Notes).HasMaxLength(500);
            entity.HasIndex(x => new { x.StudentId, x.Date }).IsUnique();

            entity.HasOne(x => x.Student)
                .WithMany()
                .HasForeignKey(x => x.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.MarkedBy)
                .WithMany()
                .HasForeignKey(x => x.MarkedByUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.ToTable("Subjects");
            entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
            entity.HasIndex(x => x.Name).IsUnique();
            entity.Property(x => x.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<PeriodAttendanceRecord>(entity =>
        {
            entity.ToTable("PeriodAttendanceRecords");
            entity.Property(x => x.Class).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Section).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Notes).HasMaxLength(500);
            entity.HasIndex(x => new { x.StudentId, x.Date, x.HourNumber, x.SubjectId }).IsUnique();

            entity.HasOne(x => x.Student).WithMany().HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Subject).WithMany().HasForeignKey(x => x.SubjectId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.TeacherUser).WithMany().HasForeignKey(x => x.TeacherUserId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.MarkedBy).WithMany().HasForeignKey(x => x.MarkedByUserId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.ToTable("Teachers");
            entity.Property(x => x.FullName).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Phone).HasMaxLength(30);
            entity.Property(x => x.Email).HasMaxLength(256);
            entity.Property(x => x.IsActive).HasDefaultValue(true);
            entity.HasIndex(x => x.UserId).IsUnique();
            entity.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TeacherClassSubjectAllocation>(entity =>
        {
            entity.ToTable("TeacherClassSubjectAllocations");
            entity.Property(x => x.Class).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Section).HasMaxLength(50).IsRequired();
            entity.Property(x => x.IsActive).HasDefaultValue(true);
            entity.HasIndex(x => new { x.TeacherId, x.SubjectId, x.Class, x.Section }).IsUnique();

            entity.HasOne(x => x.Teacher).WithMany().HasForeignKey(x => x.TeacherId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Subject).WithMany().HasForeignKey(x => x.SubjectId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.ToTable("Courses");
            entity.Property(x => x.Name).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Code).HasMaxLength(30);
            entity.Property(x => x.IsActive).HasDefaultValue(true);
            entity.HasIndex(x => x.Name).IsUnique();
            entity.HasIndex(x => x.Code).IsUnique().HasFilter("[Code] IS NOT NULL");
        });

        modelBuilder.Entity<CourseSection>(entity =>
        {
            entity.ToTable("CourseSections");
            entity.Property(x => x.Section).HasMaxLength(50).IsRequired();
            entity.Property(x => x.IsActive).HasDefaultValue(true);
            entity.HasIndex(x => new { x.CourseId, x.Section }).IsUnique();
            entity.HasOne(x => x.Course).WithMany().HasForeignKey(x => x.CourseId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CourseSubject>(entity =>
        {
            entity.ToTable("CourseSubjects");
            entity.Property(x => x.IsActive).HasDefaultValue(true);
            entity.HasIndex(x => new { x.CourseId, x.SubjectId }).IsUnique();
            entity.HasOne(x => x.Course).WithMany().HasForeignKey(x => x.CourseId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Subject).WithMany().HasForeignKey(x => x.SubjectId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TimetableEntry>(entity =>
        {
            entity.ToTable("TimetableEntries");
            entity.Property(x => x.Class).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Section).HasMaxLength(50).IsRequired();
            entity.Property(x => x.StartTime).HasColumnType("time").IsRequired();
            entity.Property(x => x.EndTime).HasColumnType("time").IsRequired();
            entity.Property(x => x.IsActive).HasDefaultValue(true);
            entity.HasIndex(x => new { x.Class, x.Section, x.DayOfWeek, x.StartTime, x.EndTime }).IsUnique();
            entity.HasIndex(x => new { x.TeacherId, x.DayOfWeek, x.StartTime, x.EndTime }).IsUnique();

            entity.HasOne(x => x.Subject).WithMany().HasForeignKey(x => x.SubjectId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Teacher).WithMany().HasForeignKey(x => x.TeacherId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}

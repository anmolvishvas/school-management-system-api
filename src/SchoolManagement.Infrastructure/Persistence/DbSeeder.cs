using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext db,
        bool seedDemoStudents,
        CancellationToken cancellationToken = default)
    {
        if (!await db.Users.AnyAsync(cancellationToken))
        {
            db.Users.Add(new User
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = ApplicationRoles.Admin
            });

            db.Users.Add(new User
            {
                Username = "teacher1",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher@123"),
                Role = ApplicationRoles.Teacher
            });

            await db.SaveChangesAsync(cancellationToken);
        }

        if (seedDemoStudents && !await db.Students.AnyAsync(cancellationToken))
        {
            db.Students.AddRange(
                new Student
                {
                    Name = "Aarav Sharma",
                    Class = "10",
                    Section = "A",
                    AdmissionNumber = "ADM-1001",
                    IsActive = true
                },
                new Student
                {
                    Name = "Ishita Verma",
                    Class = "10",
                    Section = "A",
                    AdmissionNumber = "ADM-1002",
                    IsActive = true
                });

            await db.SaveChangesAsync(cancellationToken);
        }
    }
}

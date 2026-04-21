using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db, CancellationToken cancellationToken = default)
    {
        if (await db.Users.AnyAsync(cancellationToken))
            return;

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
}

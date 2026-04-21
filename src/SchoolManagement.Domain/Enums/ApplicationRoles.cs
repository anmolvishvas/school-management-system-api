namespace SchoolManagement.Domain.Enums;

public static class ApplicationRoles
{
    public const string Admin = "Admin";
    public const string Teacher = "Teacher";
    public const string Student = "Student";
    public const string Accountant = "Accountant";

    public static readonly IReadOnlyCollection<string> All =
        new[] { Admin, Teacher, Student, Accountant };
}

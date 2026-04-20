using SchoolManagementAPI.Data;
using SchoolManagementAPI.Models;
using SchoolManagementAPI.DTOs;

namespace SchoolManagementAPI.Services
{
    public class StudentService : IStudentService
    {
        private readonly ApplicationDbContext _context;

        public StudentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<StudentDto> GetAll()
        {
            return _context.Students
                .Select(s => new StudentDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Class = s.Class,
                    Section = s.Section
                }).ToList();
        }

        public StudentDto? GetById(int id)
        {
            var s = _context.Students.Find(id);
            if (s == null) return null;

            return new StudentDto
            {
                Id = s.Id,
                Name = s.Name,
                Class = s.Class,
                Section = s.Section
            };
        }

        public StudentDto Create(CreateStudentDto dto)
        {
            var student = new Student
            {
                Name = dto.Name,
                Class = dto.Class,
                Section = dto.Section
            };

            _context.Students.Add(student);
            _context.SaveChanges();

            return new StudentDto
            {
                Id = student.Id,
                Name = student.Name,
                Class = student.Class,
                Section = student.Section
            };
        }

        public StudentDto? Update(int id, UpdateStudentDto dto)
        {
            var s = _context.Students.Find(id);
            if (s == null) return null;

            s.Name = dto.Name;
            s.Class = dto.Class;
            s.Section = dto.Section;

            _context.SaveChanges();

            return new StudentDto
            {
                Id = s.Id,
                Name = s.Name,
                Class = s.Class,
                Section = s.Section
            };
        }

        public bool Delete(int id)
        {
            var student = _context.Students.Find(id);
            if (student == null) return false;

            _context.Students.Remove(student);
            _context.SaveChanges();
            return true;
        }
    }
}
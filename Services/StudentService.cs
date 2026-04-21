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

        public object GetPaged(int page, int pageSize, string search, string sortBy, string order, string className, string section)
        {
            var query = _context.Students.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s =>
                    s.Name.Contains(search) ||
                    s.Class.Contains(search) ||
                    s.Section.Contains(search)
                );
            }

            if (!string.IsNullOrEmpty(className))
            {
                query = query.Where(s =>
                    s.Class != null &&
                    s.Class.ToLower().Trim() == className.ToLower().Trim()
                );
            }

            if (!string.IsNullOrEmpty(section))
            {
                query = query.Where(s =>
                    s.Section != null &&
                    s.Section.ToLower().Trim() == section.ToLower().Trim()
                );
            }

            bool isAsc = order?.ToLower() != "desc";

            query = sortBy?.ToLower() switch
            {
                "name" => isAsc ? query.OrderBy(s => s.Name) : query.OrderByDescending(s => s.Name),
                "class" => isAsc ? query.OrderBy(s => s.Class) : query.OrderByDescending(s => s.Class),
                "section" => isAsc ? query.OrderBy(s => s.Section) : query.OrderByDescending(s => s.Section),
                _ => query.OrderBy(s => s.Id)
            };

            var total = query.Count();

            var data = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new StudentDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Class = s.Class,
                    Section = s.Section
                })
                .ToList();

            return new { total, page, pageSize, data };
        }

        public object GetStats()
        {
            var totalStudents = _context.Students.Count();

            var byClass = _context.Students
                .GroupBy(s => s.Class)
                .Select(g => new { label = g.Key, count = g.Count() })
                .ToList();

            var bySection = _context.Students
                .GroupBy(s => s.Section)
                .Select(g => new { label = g.Key, count = g.Count() })
                .ToList();

            return new
            {
                totalStudents,
                byClass,
                bySection
            };
        }
    }
}
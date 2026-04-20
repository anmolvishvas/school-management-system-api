using SchoolManagementAPI.DTOs;

public interface IStudentService
{
    List<StudentDto> GetAll();
    StudentDto? GetById(int id);
    StudentDto Create(CreateStudentDto dto);
    StudentDto? Update(int id, UpdateStudentDto dto);
    bool Delete(int id);
}
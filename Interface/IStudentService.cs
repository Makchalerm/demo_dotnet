// Services/IStudentService.cs
using MyApiProject.Models;

namespace MyApiProject.Services
{
    public interface IStudentService
    {
        Task<List<Student>> GetAllAsync();
        Task<Student?> GetByIdAsync(int id);
        Task<Student?> CreateAsync(Student student);
        Task<List<Student>> CreateManyAsync(List<Student> students);
        Task<Student?> UpdateAsync(int id, Student student);
        Task<Student?> DeleteAsync(int id);
        Task<List<Student?>> DeleteAllAsync();
        Task<bool> ExistsByFullNameAsync(string firstName, string lastName);
        Task<List<Student>> SearchAsync(string? firstName, string? lastName, string? major);

    }
}

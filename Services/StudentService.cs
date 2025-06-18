// Services/StudentService.cs
using Microsoft.EntityFrameworkCore;
using MyApiProject.Data;
using MyApiProject.Models;

namespace MyApiProject.Services
{
    public class StudentService : IStudentService
    {
        private readonly ApplicationDbContext _context;

        public StudentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Student>> GetAllAsync()
        {
            return await _context.Students.ToListAsync();
        }
        public async Task<Student?> GetByIdAsync(int id)
        {
            return await _context.Students.FindAsync(id);
        }
        public async Task<bool> ExistsByFullNameAsync(string firstName, string lastName)
        {
            return await _context.Students.AnyAsync(s =>
                s.FirstName.ToLower() == firstName.ToLower() &&
                s.LastName.ToLower() == lastName.ToLower()
            );
        }
        public async Task<Student?> CreateAsync(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return student;
        }
        public async Task<List<Student>> CreateManyAsync(List<Student> students)
        {
            _context.Students.AddRange(students);
            await _context.SaveChangesAsync();
            return students;
        }
        public async Task<Student?> UpdateAsync(int id, Student student)
        {
            var existingStudent = await _context.Students.FindAsync(id);
            if (existingStudent == null)
            {
                return null; // Student not found
            }

            existingStudent.FirstName = student.FirstName;
            existingStudent.LastName = student.LastName;
            existingStudent.DateOfBirth = student.DateOfBirth;
            existingStudent.Major = student.Major;

            await _context.SaveChangesAsync();
            return existingStudent;
        }
        public async Task<Student?> DeleteAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return null; // Student not found
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return student;
        }
        public async Task<List<Student>> DeleteAllAsync()
        {
            var students = await _context.Students.ToListAsync();
            _context.Students.RemoveRange(students);
            await _context.SaveChangesAsync();
            return students;
        }
    }
}

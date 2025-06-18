// Controllers/StudentController.cs
using Microsoft.AspNetCore.Mvc;
using MyApiProject.Services;
using MyApiProject.Models; // ✅ ให้เจอ Student
using Microsoft.EntityFrameworkCore; // ✅ ให้เจอ DbUpdateException


namespace MyApiProject.Controllers
{
    [ApiController]
    [Route("api/std")]

    public class StudentController : ControllerBase
    {
        private readonly IStudentService _service;

        public StudentController(IStudentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                if (_service == null)
                {
                    return StatusCode(500, "❌ Service is not initialized.");
                }
                var students = await _service.GetAllAsync();
                return Ok(students);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"❌ General Error: {ex.Message}");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Student student)
        {
            try
            {
                if (student == null)
                {
                    return BadRequest("❌ Invalid student data.");
                }
                var created = await _service.CreateAsync(student);
                return CreatedAtAction(nameof(Get), new { id = created.StudentId }, created);
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, $"❌ DB Error: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"❌ General Error: {ex.Message}");
            }
        }
        [HttpPost("bulk")]
        public async Task<IActionResult> CreateMany([FromBody] List<Student> students)
        {
            try
            {
                if (students == null || !students.Any())
                {
                    return BadRequest("❌ Invalid student data.");
                }
                var created = await _service.CreateManyAsync(students);
                return Ok(created);
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, $"❌ DB Error: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"❌ General Error: {ex.Message}");
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Student student)
        {
            try
            {
                if (student == null || id <= 0)
                {
                    return BadRequest("❌ Invalid student data.");
                }
                var updated = await _service.UpdateAsync(id, student);
                if (updated == null)
                {
                    return NotFound($"❌ Student with ID {id} not found.");
                }
                return Ok(updated);
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, $"❌ DB Error: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"❌ General Error: {ex.Message}");
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("❌ Invalid student ID.");
                }
                var deleted = await _service.DeleteAsync(id);
                if (deleted == null)
                {
                    return NotFound($"❌ Student with ID {id} not found.");
                }
                return Ok(deleted);
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, $"❌ DB Error: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"❌ General Error: {ex.Message}");
            }
        }
    }
}
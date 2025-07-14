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
        // GET api/std http://localhost:5175/api/std
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                if (_service == null)
                {
                    return StatusCode(500, new
                    {
                        status = 500,
                        message = "❌ Service is not initialized."
                    });
                }

                var students = await _service.GetAllAsync();
                return Ok((new
                {
                    status = 200,
                    message = "✅ Student found.",
                    Data = students
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "❌ Unexpected error.",
                    detail = ex.Message
                });
            }
        }
        // GET api/std/1 http://localhost:5175/api/std/1
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new
                    {
                        status = 400,
                        message = "❌ Invalid student ID."
                    });
                }
                var student = await _service.GetByIdAsync(id);
                if (student == null)
                {
                    return BadRequest(new
                    {
                        status = 404,
                        message = $"❌ Student ID {id} not found"
                    });
                }
                return Ok(new
                {
                    status = 200,
                    message = "✅ Student found.",
                    data = student
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "❌ Unexpected error.",
                    detail = ex.Message
                });
            }
        }
        // GET api/std/search?firstName=...&lastName=...&major=...
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? firstName, [FromQuery] string? lastName, [FromQuery] string? major)
        {
            try
            {
                var results = await _service.SearchAsync(firstName, lastName, major);

                if (results == null || !results.Any())
                {
                    return NotFound(new
                    {
                        status = 404,
                        message = "❌ No students found matching the criteria.",
                        data = new List<Student>()
                    });
                }

                return Ok(new
                {
                    status = 200,
                    message = $"✅ Found {results.Count} student(s).",
                    data = results
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "❌ Search failed due to unexpected error.",
                    detail = ex.Message
                });
            }
        }


        // POST api/std http://localhost:5175/api/std
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Student student)
        {
            try
            {
                // === ตรวจข้อมูลซ้ำ ===
                bool exists = await _service.ExistsByFullNameAsync(student.FirstName, student.LastName);
                if (exists)
                {
                    return Conflict(new
                    {
                        status = 409,
                        message = "❌ Student already exists.",
                        detail = new { student.FirstName, student.LastName }
                    });
                }

                // === สร้างใหม่ ===
                var created = await _service.CreateAsync(student);
                return CreatedAtAction(nameof(Get), new { id = created.StudentId }, new
                {
                    status = 200,
                    message = "✅ Student created successfully.",
                    data = created
                });
            }
            catch (InvalidOperationException invEx)
            {
                return Conflict(new
                {
                    status = 409,
                    message = "❌ Invalid operation.",
                    detail = invEx.Message
                });
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "❌ Database error.",
                    detail = dbEx.InnerException?.Message ?? dbEx.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "❌ Unexpected error.",
                    detail = ex.Message
                });
            }
        }
        // POST api/std/bulk http://localhost:5175/api/std/bulk
        [HttpPost("bulk")]
        public async Task<IActionResult> CreateMany([FromBody] List<Student> students)
        {
            try
            {
                if (students == null || !students.Any())
                {
                    return BadRequest(new
                    {
                        status = 400,
                        message = "❌ No student data provided."
                    });
                }

                // ตรวจสอบชื่อซ้ำในฐานข้อมูล (อย่างง่าย: เช็ค FirstName + LastName ซ้ำ)
                var duplicates = new List<Student>();

                foreach (var s in students)
                {
                    bool exists = await _service.ExistsByFullNameAsync(s.FirstName, s.LastName);
                    if (exists)
                    {
                        duplicates.Add(s);
                    }
                }

                if (duplicates.Any())
                {
                    return Conflict(new
                    {
                        status = 409,
                        message = $"❌ {duplicates.Count} students duplicate.",
                        duplicates = duplicates.Select(d => new { d.FirstName, d.LastName })
                    });
                }

                var createdStudents = await _service.CreateManyAsync(students);

                return Created("api/std/bulk", new
                {
                    status = 200,
                    message = $"✅ {createdStudents.Count} students created successfully.",
                    data = createdStudents
                });
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "❌ Database error.",
                    detail = dbEx.InnerException?.Message ?? dbEx.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "❌ Unexpected error.",
                    detail = ex.Message
                });
            }
        }
        // PUT api/std/1 http://localhost:5175/api/std/1
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Student student)
        {
            try
            {
                if (student == null || id <= 0)
                {
                    return BadRequest(new
                    {
                        status = 400,
                        message = "❌ Invalid student data."
                    });
                }

                var updated = await _service.UpdateAsync(id, student);
                if (updated == null)
                {
                    return NotFound(new
                    {
                        status = 400,
                        message = $"❌ Student with ID {id} not found."
                    });
                }
                return Ok((new
                {
                    status = 200,
                    message = "✅ Student updated successfully.",
                    data = updated
                }));
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "❌ Database error.",
                    details = dbEx.InnerException?.Message ?? dbEx.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "❌ Unexpected error.",
                    detail = ex.Message
                });
            }
        }
        // DELETE api/std/1 http://localhost:5175/api/std/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new
                    {
                        status = 400,
                        message = "❌ Invalid student ID."
                    });
                }

                var deleted = await _service.DeleteAsync(id);
                if (deleted == null)
                {
                    return NotFound(new
                    {
                        status = 404,
                        message = $"❌ Student with ID {id} not found."
                    });
                }

                return Ok(new
                {
                    status = 200,
                    message = "✅ Student deleted successfully.",
                    data = deleted
                });
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "❌ Database error.",
                    detail = dbEx.InnerException?.Message ?? dbEx.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "❌ Unexpected error.",
                    detail = ex.Message
                });
            }
        }

        // DELETE api/std/all http://localhost:5175/api/std/all
        [HttpDelete("all")]
        public async Task<IActionResult> DeleteAll()
        {
            try
            {
                var deletedStudents = await _service.DeleteAllAsync();

                return Ok(new
                {
                    status = 200,
                    message = $"✅ Deleted {deletedStudents.Count} students successfully.",
                    data = deletedStudents
                });
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "❌ Database error.",
                    detail = dbEx.InnerException?.Message ?? dbEx.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "❌ Unexpected error.",
                    detail = ex.Message
                });
            }
        }
    }
}
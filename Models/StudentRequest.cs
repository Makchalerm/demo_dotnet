// Models/Student.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApiProject.Models
{
    public class StudentRequest
    {
        [Column("student_id")]
        public int StudentId { get; set; }

        [Column("first_name")]
        [StringLength(50)]
        public string? FirstName { get; set; }

        [Column("last_name")]
        [StringLength(50)]
        public string? LastName { get; set; }

        [Column("date_of_birth")]
        public DateTime DateOfBirth { get; set; }

        [Column("major")]
        [StringLength(100)]
        public string? Major { get; set; }

        [Column("username")]
        [StringLength(50)]
        public string? Username { get; set; }

        [Column("password")]
        [StringLength(255)]
        public string? Password { get; set; }
    }
}


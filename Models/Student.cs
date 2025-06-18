// Models/Student.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApiProject.Models
{
    [Table("student")]
    public class Student
    {
        [Key]
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
    }
}


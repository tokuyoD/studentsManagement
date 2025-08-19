using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagement.Models
{
    public class Student
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // 自動遞增 Id 
        public int Id { get; set; }

        [Required(ErrorMessage = "姓名必填")]
        public string Name { get; set; }

        [Required(ErrorMessage = "年齡必填")]
        [Range(1, 120, ErrorMessage = "年齡必須介於 1 到 120")]
        public int Age { get; set; }

        [EmailAddress(ErrorMessage = "Email 格式不正確")]
        public string Email { get; set; }
        public string Gender { get; set; }
        public string Class { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
        
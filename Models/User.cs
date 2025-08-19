using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagement.Models
{
    public class User
    {
        [Key]
        [Column("UserId")]
        public int UserId { get; set; }

        [Required]
        [Column("Username")]
        [MaxLength(50)]
        public string UserName { get; set; }

        [Required]
        [Column("PasswordHash")]
        [MaxLength(256)]
        public string PasswordHash { get; set; }

        [Required]
        [Column("Name")]
        [MaxLength(100)]
        public string Name { get; set; }

        [Column("Email")]
        [MaxLength(100)]
        public string Email { get; set; }

        [Column("CreatedAt")]
        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        public ICollection<UserRole> UserRoles { get; set; }
    }
}

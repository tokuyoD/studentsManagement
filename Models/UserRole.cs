using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagement.Models
{
    [Table("UserRoles")] // 對應資料表名稱
    public class UserRole
    {
        [Key]
        public int Id { get; set; }

        // 對應 User
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        // 對應 Role
        [ForeignKey("Role")]
        public int RoleId { get; set; }
        public Role Role { get; set; }  // Navigation property
    }
}

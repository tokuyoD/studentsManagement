using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagement.Models
{
    [Table("Role")] // 對應資料表名稱
    public class Role
    {
        [Key]
        [Column("RoleId")]  // 對應 DB 欄位 RoleId
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column("RoleName")] // 對應 DB 欄位 RoleName
        [MaxLength(50)]
        public string Name { get; set; }

        // 導覽屬性，對應 UserRole 關聯
        public ICollection<UserRole> UserRoles { get; set; }
    }
}

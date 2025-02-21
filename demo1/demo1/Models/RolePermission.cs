using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
namespace demo1.Models
{
    public class RolePermission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string RoleId { get; set; }= null!;

        [Required]
        public int PermissionId { get; set; }

        [ForeignKey("RoleId")]
        public virtual IdentityRole Role { get; set; }= null!;

        [ForeignKey("PermissionId")]
        public virtual Permission Permission { get; set; }= null!;
    }
}
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
        public string UserId { get; set; } = null!;

        [Required]
        public string RoleId { get; set; } = null!;

        [Required]
        public bool CanView { get; set; } = false;

        [Required]
        public bool CanInsert { get; set; } = false;

        [Required]
        public bool CanUpdate { get; set; } = false;

        [Required]
        public bool CanDelete { get; set; } = false;

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;

        [ForeignKey("RoleId")]
        public virtual IdentityRole Role { get; set; } = null!;
    }
}
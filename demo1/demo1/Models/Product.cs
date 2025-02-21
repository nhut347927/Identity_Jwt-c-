using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace demo1.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Tự động tăng
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }= null!;

        [Required]
        public decimal Price { get; set; }
    }
}
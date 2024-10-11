using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductsAPI.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(9)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(90)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(28)]
        public string Category { get; set; } = string.Empty;

        [StringLength(28)]
        public string? Brand { get; set; }

        [StringLength(21)]
        public string? Type { get; set; }

        [StringLength(180)]
        public string? Description { get; set; }

        [Column("created_at")] // Map to the database column
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")] // Map to the database column
        public DateTime UpdatedAt { get; set; }

    }
}
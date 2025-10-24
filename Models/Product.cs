using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    [Table("Products")]
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Code { get; set; }

        [StringLength(255)]
        public string? ImageURL { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Cost price must be positive")]
        public decimal? CostPrice { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Retail price must be positive")]
        public decimal? RetailPrice { get; set; }

        public DateTime? CreationDate { get; set; }

        // Fix: Changed from UpdateDate to UpdatedDate to match database
        public DateTime? UpdatedDate { get; set; }
    }
}
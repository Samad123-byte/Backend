using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    [Table("SaleDetails")] // Updated to match the renamed table
    public class SaleDetail
    {
        [Key]
        public int SaleDetailId { get; set; }

        [Required]
        public int SaleId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Retail price must be positive")]
        public decimal RetailPrice { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100")]
        public decimal? Discount { get; set; }

        // Navigation properties (optional, for Entity Framework relationships)
        // public virtual Sale? Sale { get; set; }
        // public virtual Product? Product { get; set; }

        // Calculated properties (not mapped to database)
        [NotMapped]
        public decimal SubTotal => RetailPrice * Quantity;

        [NotMapped]
        public decimal DiscountAmount => SubTotal * (Discount ?? 0) / 100;

        [NotMapped]
        public decimal Total => SubTotal - DiscountAmount;
    }
}
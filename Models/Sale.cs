using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    [Table("Sales")] // Fixed: Changed from "SalesMaster" to "Sales" to match stored procedures
    public class Sale
    {
        [Key]
        public int SaleId { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Total must be positive")]
        public decimal Total { get; set; }

        [Required]
        public DateTime SaleDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? SalespersonId { get; set; }

        [StringLength(500)] // Updated to match stored procedure parameter length
        public string? Comments { get; set; }

        public DateTime? UpdatedDate { get; set; }

        // Navigation property to SaleDetails
        public virtual ICollection<SaleDetail>? SaleDetails { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    [Table("SalesMaster")]
    public class Sale
    {
        [Key]
        public int SaleId { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Total must be positive")]
        public decimal Total { get; set; }

        [Required]
        public DateTime SaleDate { get; set; }

        public int? SalespersonId { get; set; }

        [StringLength(255)]
        public string? Comments { get; set; }

        public DateTime? UpdatedDate { get; set; }

        // Navigation property to SaleDetails
        public virtual ICollection<SaleDetail>? SaleDetails { get; set; }
    }
}
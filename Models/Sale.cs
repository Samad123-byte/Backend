using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    [Table("Sales")]
    public class Sale
    {
        [Key]
        public int SaleId { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Total must be positive")]
        public decimal Total { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? SaleDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? SalespersonId { get; set; }

        [StringLength(500)]
        public string? Comments { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [NotMapped]
        public string? SalespersonName { get; set; }

        [NotMapped]
        public List<SaleDetailDto>? SaleDetails { get; set; }

        public class SaleDetailDto
        {
            public int ProductId { get; set; }
            public decimal RetailPrice { get; set; }
            public int Quantity { get; set; }
            public decimal? Discount { get; set; }

            public string RowState { get; set; } = "Unchanged";
        }

    }
}

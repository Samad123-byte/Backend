using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    [Table("Salesperson")]
    public class Salesperson
    {
        [Key]
        public int SalespersonId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50, ErrorMessage = "Code cannot exceed 50 characters")]
        public string Code { get; set; } = string.Empty;

        public DateTime? EnteredDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        // Navigation property to Sales
        public virtual ICollection<Sale>? Sales { get; set; }
    }
}

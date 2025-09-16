using System.ComponentModel.DataAnnotations;

namespace ContractMonthlyClaimSystem.Models
{
    public class ClaimItem
    {
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [Range(0.1, 24, ErrorMessage = "Hours must be between 0.1 and 24")]
        public decimal HoursWorked { get; set; }

        [Required]
        [StringLength(50)]
        public string Module { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        public decimal Amount { get; set; }
    }
}
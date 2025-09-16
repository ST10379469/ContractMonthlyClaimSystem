using System.ComponentModel.DataAnnotations;

namespace ContractMonthlyClaimSystem.Models
{
    public class MonthlyClaim
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        [Range(1, 12)]
        public int Month { get; set; }

        [Required]
        [Range(2020, 2030)]
        public int Year { get; set; }

        public decimal TotalAmount { get; set; }

        public ClaimStatus Status { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? SubmittedDate { get; set; }

        public List<ClaimItem> ClaimItems { get; set; } = new List<ClaimItem>();
        public List<SupportingDocument> SupportingDocuments { get; set; } = new List<SupportingDocument>();
    }
}
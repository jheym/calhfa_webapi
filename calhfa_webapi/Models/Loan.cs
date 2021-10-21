using System;
using System.Collections.Generic;

#nullable disable

namespace calhfa_webapi.Models
{
    public partial class Loan
    {
        public Loan()
        {
            LoanStatuses = new HashSet<LoanStatus>();
        }

        public int LoanId { get; set; }
        public int? LoanTypeId { get; set; }
        public decimal? LoanAmount { get; set; }
        public decimal? AnnualIncome { get; set; }
        public int? HouseholdCount { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public decimal? Lvratio { get; set; }
        public decimal? InterestRate { get; set; }
        public decimal? Balance { get; set; }
        public int? InsCode { get; set; }
        public string InsurerNum { get; set; }
        public DateTime? ReservDateTime { get; set; }

        public virtual LoanType LoanType { get; set; }
        public virtual ICollection<LoanStatus> LoanStatuses { get; set; }
    }
}

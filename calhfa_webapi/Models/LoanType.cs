using System;
using System.Collections.Generic;

#nullable disable

namespace CalhfaWebapi.Models
{
    public partial class LoanType
    {
        public LoanType()
        {
            Loans = new HashSet<Loan>();
        }

        public int LoanTypeId { get; set; }
        public int LoanCategoryId { get; set; }
        public string Description { get; set; }
        public string LongDescription { get; set; }
        public bool? Active { get; set; }
        public DateTime? ActiveEndDate { get; set; }
        public int RateLockDays { get; set; }
        public bool ActiveRates { get; set; }
        public int? LoanTypeCategoryId { get; set; }
        public decimal? SortOrder { get; set; }
        public int? ProductTypeId { get; set; }

        public virtual LoanCategory LoanCategory { get; set; }
        public virtual ICollection<Loan> Loans { get; set; }
    }
}

using System;
using System.Collections.Generic;

#nullable disable

namespace CalhfaWebapi.Models
{
    public partial class LoanStatus
    {
        public int LoanStatusId { get; set; }
        public int LoanId { get; set; }
        public int? StatusCode { get; set; }
        public DateTime? StatusDate { get; set; }
        public string LoginName { get; set; }
        public DateTime? LoginDate { get; set; }
        public int? StatusSequence { get; set; }

        public virtual Loan Loan { get; set; }
        public virtual StatusCode StatusCodeNavigation { get; set; }
    }
}

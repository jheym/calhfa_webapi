using System;
using System.Collections.Generic;

#nullable disable

namespace CalhfaWebapi.Models
{
    public partial class StatusCode
    {
        public StatusCode()
        {
            LoanStatuses = new HashSet<LoanStatus>();
        }

        public int StatusCode1 { get; set; }
        public string Description { get; set; }
        public string BusinessUnit { get; set; }
        public string NotesAndAssumptions { get; set; }
        public int? ConversationCategoryId { get; set; }

        public virtual ICollection<LoanStatus> LoanStatuses { get; set; }
    }
}

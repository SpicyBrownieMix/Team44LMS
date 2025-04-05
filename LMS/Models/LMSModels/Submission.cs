using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Submission
    {
        public uint AssignmentId { get; set; }
        public string Student { get; set; } = null!;
        public DateTime SubmissionTime { get; set; }
        public uint Score { get; set; }
        public string? Contents { get; set; }

        public virtual Assignment Assignment { get; set; } = null!;
        public virtual Student StudentNavigation { get; set; } = null!;
    }
}

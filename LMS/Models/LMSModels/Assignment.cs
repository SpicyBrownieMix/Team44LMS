using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Assignment
    {
        public Assignment()
        {
            Submissions = new HashSet<Submission>();
        }

        public uint AssignmentId { get; set; }
        public string Name { get; set; } = null!;
        public uint MaxPoints { get; set; }
        public string? Contents { get; set; }
        public DateTime DueDate { get; set; }
        public uint? Acid { get; set; }

        public virtual AssignmentCategory? Ac { get; set; }
        public virtual ICollection<Submission> Submissions { get; set; }
    }
}

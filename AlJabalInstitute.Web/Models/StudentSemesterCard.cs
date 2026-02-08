using System;

namespace AlJabalInstitute.Web.Models
{
    public class StudentSemesterCard
    {
        public Guid StudentSemesterId { get; set; }
        public Guid StudentId { get; set; }
        public string SemesterName { get; set; } = "";
        public DateTime StartDate { get; set; }

        // ✅ جديد للواجهة
        public bool IsResultVisible { get; set; }
        public bool IsFinanciallyExempt { get; set; }
        public decimal RemainingAmount { get; set; }
    }
}

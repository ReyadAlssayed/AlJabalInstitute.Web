using System;

namespace AlJabalInstitute.Web.Models
{
    public class StudentAcademicSemesterCardVM
    {
        public Guid StudentSemesterId { get; set; }
        public Guid StudentId { get; set; }      // ✅ add
        public Guid SemesterId { get; set; }

        public string SemesterName { get; set; } = "";
        public DateTime StartDate { get; set; }

        public int TotalSubjects { get; set; }
        public int PassedSubjects { get; set; }
        public int FailedSubjects { get; set; }

        public decimal TotalScore { get; set; }  // ✅ add
        public decimal Percentage { get; set; }
        public string FinalResult { get; set; } = "—";

        public bool IsVisible { get; set; }
        public bool IsResultsPublished { get; set; }
        public bool IsFinanciallyExempt { get; set; }
        public decimal RemainingAmount { get; set; }
        public DateTime? ResultUnlockedUntil { get; set; }
    }
}

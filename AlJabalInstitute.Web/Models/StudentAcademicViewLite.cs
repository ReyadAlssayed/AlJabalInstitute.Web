using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace AlJabalInstitute.Web.Models
{
    [Table("student_academic_view")]
    public class StudentAcademicViewLite : BaseModel
    {
        [Column("student_semester_id")]
        public Guid StudentSemesterId { get; set; }

        [Column("student_id")]
        public Guid StudentId { get; set; }

        [Column("semester_id")]
        public Guid SemesterId { get; set; }

        [Column("student_name")]
        public string? StudentName { get; set; }

        [Column("semester_name")]
        public string? SemesterName { get; set; }

        [Column("start_date")]
        public DateTime StartDate { get; set; }

        // ===== الأكاديمية =====
        [Column("total_subjects")]
        public int? TotalSubjects { get; set; }

        [Column("passed_subjects")]
        public int? PassedSubjects { get; set; }

        [Column("failed_subjects")]
        public int? FailedSubjects { get; set; }

        [Column("total_score")]
        public decimal? TotalScore { get; set; }

        [Column("percentage")]
        public decimal? Percentage { get; set; }

        [Column("final_result")]
        public string? FinalResult { get; set; }

        // ===== منطق الإظهار =====
        [Column("is_results_published")]
        public bool? IsResultsPublished { get; set; }

        [Column("is_financially_exempt")]
        public bool IsFinanciallyExempt { get; set; }

        [Column("remaining_amount")]
        public decimal RemainingAmount { get; set; }

        [Column("result_unlocked_until")]
        public DateTime? ResultUnlockedUntil { get; set; }

        // (اختياري) لا تعتمد عليه للحكم
        [Column("is_result_visible")]
        public bool IsResultVisible { get; set; }
    }
}

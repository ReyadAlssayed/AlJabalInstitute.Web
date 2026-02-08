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

        [Column("semester_name")]
        public string? SemesterName { get; set; }

        [Column("start_date")]
        public DateTime StartDate { get; set; }

        // ✅ جديد للواجهة
        [Column("is_result_visible")]
        public bool IsResultVisible { get; set; }

        [Column("is_financially_exempt")]
        public bool IsFinanciallyExempt { get; set; }

        [Column("remaining_amount")]
        public decimal RemainingAmount { get; set; }
    }
}

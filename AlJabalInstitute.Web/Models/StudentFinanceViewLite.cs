using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace AlJabalInstitute.Web.Models
{
    [Table("student_finance_view")]
    public class StudentFinanceViewLite : BaseModel
    {
        [Column("student_semester_id")]
        public Guid? StudentSemesterId { get; set; }

        [Column("student_id")]
        public Guid? StudentId { get; set; }

        [Column("semester_id")]
        public Guid? SemesterId { get; set; }

        [Column("student_name")]
        public string? StudentName { get; set; }

        [Column("semester_name")]
        public string? SemesterName { get; set; }

        [Column("start_date")]
        public DateTime? StartDate { get; set; }

        [Column("semester_price")]
        public decimal? SemesterPrice { get; set; }

        [Column("discount_amount")]
        public decimal? DiscountAmount { get; set; }

        [Column("paid_amount")]
        public decimal? PaidAmount { get; set; }

        [Column("remaining_amount")]
        public decimal? RemainingAmount { get; set; }

        [Column("surplus_amount")]
        public decimal? SurplusAmount { get; set; }

        [Column("is_financially_exempt")]
        public bool? IsFinanciallyExempt { get; set; }
    }
}

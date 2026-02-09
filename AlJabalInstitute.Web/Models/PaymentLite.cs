using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace AlJabalInstitute.Web.Models
{
    [Table("payments")]
    public class PaymentLite : BaseModel
    {
        [PrimaryKey("id", false)]
        public Guid Id { get; set; }

        [Column("student_semester_id")]
        public Guid StudentSemesterId { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

        // Enum في قاعدة البيانات (payment_method_enum)
        // نخليه string لسهولة القراءة في الواجهة
        [Column("payment_method")]
        public string? PaymentMethod { get; set; }

        [Column("paid_at")]
        public DateTime? PaidAt { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("receiver_name")]
        public string? ReceiverName { get; set; }

        [Column("payment_note")]
        public string? PaymentNote { get; set; }
    }
}

public class StudentSemesterCard
{
    public Guid StudentSemesterId { get; set; }
    public Guid StudentId { get; set; }
    public string SemesterName { get; set; } = "";
    public DateTime StartDate { get; set; }

    // ✅ للمالية
    public bool IsFinanciallyExempt { get; set; }
    public decimal RemainingAmount { get; set; }

    // ✅ للأكاديمية (من view)
    public bool? IsResultsPublished { get; set; }

    public DateTime? ResultUnlockedUntil { get; set; }

    // (اختياري) لا تستخدمه في الحكم
    public bool IsResultVisible { get; set; }
}

using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace AlJabalInstitute.Web.Models
{
    [Table("students")]
    public class Student : BaseModel
    {
        [PrimaryKey("id", false)]
        public Guid Id { get; set; }

        [Column("full_name")]
        public string FullName { get; set; } = "";

        [Column("national_id")]
        public string NationalId { get; set; } = "";

        [Column("password")]
        public string? Password { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }

        [Column("last_login_at")]
        public DateTime? LastLoginAt { get; set; }
    }
}

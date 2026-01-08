using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace AlJabalInstitute.Web.Models
{
    [Table("semesters")]
    public class Semesters : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }

        [Column("semestername")]
        public string SemesterName { get; set; } = string.Empty;

        [Column("startdate")]
        public DateTime StartDate { get; set; }

        [Column("enddate")]
        public DateTime? EndDate { get; set; }

        [Column("isfinished")]
        public bool IsFinished { get; set; }

        // ⭐ أضف هذا السطر
        [Column("teacher_id")]
        public Guid? TeacherId { get; set; }
    }
}

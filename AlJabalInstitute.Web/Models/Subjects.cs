using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace AlJabalInstitute.Web.Models
{
    [Table("subjects")]
    public class Subjects : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }

        [Column("subjectname")]
        public string SubjectName { get; set; } = string.Empty;

        [Column("lectureprice")]
        public decimal LecturePrice { get; set; }
    }
}

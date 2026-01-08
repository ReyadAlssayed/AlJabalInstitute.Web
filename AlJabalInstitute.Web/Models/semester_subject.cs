using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace AlJabalInstitute.Web.Models
{
    [Table("semester_subjects")]
    public class semester_subject : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }

        [Column("semester_id")]
        public Guid SemesterId { get; set; }

        [Column("subject_id")]
        public Guid SubjectId { get; set; }
    }
}

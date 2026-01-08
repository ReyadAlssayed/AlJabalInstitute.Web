using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace AlJabalInstitute.Web.Models
{
    [Table("lectures")]
    public class Lectures : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }

        [Column("lecturedate")]
        public DateTime LectureDate { get; set; }

        [Column("lectureprice")]
        public decimal LecturePrice { get; set; }

        [Column("semester_subject_id")]
        public Guid SemesterSubjectId { get; set; }

    }
}

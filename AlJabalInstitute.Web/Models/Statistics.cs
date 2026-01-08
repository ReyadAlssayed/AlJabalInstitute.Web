using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace AlJabalInstitute.Web.Models
{
    [Table("statistics")]
    public class Statistics : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }

        [Column("semesterid")]
        public Guid SemesterId { get; set; }

        [Column("totallectures")]
        public int TotalLectures { get; set; }

        [Column("totalamount")]
        public decimal TotalAmount { get; set; }

        [Column("monthlyamount")]
        public decimal MonthlyAmount { get; set; }
    }
}

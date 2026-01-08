using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace AlJabalInstitute.Web.Models
{
    [Table("teacher")]
    public class Teacher : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }

        [Column("fullname")]
        public string FullName { get; set; } = string.Empty;

        [Column("password")]
        public string Password { get; set; } = string.Empty;
    }
}

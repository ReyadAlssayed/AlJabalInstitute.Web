using System.Collections.Generic;

namespace AlJabalInstitute.Web.Models
{
    public class StudentAcademicPageVM
    {
        public string StudentName { get; set; } = "";
        public StudentAcademicViewLite? Header { get; set; }
        public List<StudentAcademicSemesterCardVM> Semesters { get; set; } = new();
    }
}

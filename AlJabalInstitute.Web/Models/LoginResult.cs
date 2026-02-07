namespace AlJabalInstitute.Web.Models
{
    public class LoginResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public Guid? StudentId { get; set; }
        public string? StudentName { get; set; }
    }
}

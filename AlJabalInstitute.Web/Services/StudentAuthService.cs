using AlJabalInstitute.Web.Models;
using Supabase;

namespace AlJabalInstitute.Web.Services
{
    public class StudentAuthService
    {
        private readonly Client _client;

        public StudentAuthService(Client client)
        {
            _client = client;
        }

        public async Task<LoginResult> Login(string nationalId, string password)
        {
            if (string.IsNullOrWhiteSpace(nationalId) ||
                string.IsNullOrWhiteSpace(password))
            {
                return new LoginResult
                {
                    Success = false,
                    Message = "يرجى إدخال الرقم الوطني وكلمة السر"
                };
            }

            var res = await _client
                .From<Student>()
                .Where(s => s.NationalId == nationalId)
                .Limit(1)
                .Get();

            var student = res.Models.FirstOrDefault();

            if (student == null || !student.IsActive)
                return new LoginResult { Success = false, Message = "بيانات الدخول غير صحيحة" };

            if (string.IsNullOrWhiteSpace(student.Password) ||
                !BCrypt.Net.BCrypt.Verify(password, student.Password))
                return new LoginResult { Success = false, Message = "بيانات الدخول غير صحيحة" };

            student.LastLoginAt = DateTime.UtcNow;

            await _client
                .From<Student>()
                .Where(s => s.Id == student.Id)
                .Set(s => s.LastLoginAt, student.LastLoginAt)
                .Update();

            return new LoginResult
            {
                Success = true,
                StudentId = student.Id,
                StudentName = student.FullName
            };
        }

        public async Task<List<StudentSemesterCard>> GetStudentSemestersAsync(Guid studentId)
        {
            var res = await _client
                .From<StudentAcademicViewLite>()
                .Where(x => x.StudentId == studentId)
                .Get();

            return res.Models
                .Select(x => new StudentSemesterCard
                {
                    StudentSemesterId = x.StudentSemesterId,
                    StudentId = x.StudentId,
                    SemesterName = x.SemesterName ?? "",
                    StartDate = x.StartDate,

                    // ✅ الحقول الجديدة
                    IsResultVisible = x.IsResultVisible,
                    IsFinanciallyExempt = x.IsFinanciallyExempt,
                    RemainingAmount = x.RemainingAmount
                })
                .OrderByDescending(x => x.StartDate)
                .ToList();
        }

    }
}

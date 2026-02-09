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
            if (string.IsNullOrWhiteSpace(nationalId) || string.IsNullOrWhiteSpace(password))
            {
                return new LoginResult { Success = false, Message = "يرجى إدخال الرقم الوطني وكلمة السر" };
            }

            try
            {
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
            catch (HttpRequestException)
            {
                return new LoginResult
                {
                    Success = false,
                    Message = "لا يوجد اتصال بالإنترنت. تأكد من الشبكة ثم حاول مرة أخرى."
                };
            }
            catch
            {
                return new LoginResult
                {
                    Success = false,
                    Message = "حدث خطأ غير متوقع. حاول لاحقًا."
                };
            }
        }

        public async Task<List<StudentSemesterCard>> GetStudentSemestersAsync(Guid studentId)
        {
            try
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

                        IsFinanciallyExempt = x.IsFinanciallyExempt,
                        RemainingAmount = x.RemainingAmount,

                        IsResultsPublished = x.IsResultsPublished,
                        ResultUnlockedUntil = x.ResultUnlockedUntil,

                        IsResultVisible = x.IsResultVisible
                    })
                    .OrderByDescending(x => x.StartDate)
                    .ToList();
            }
            catch (HttpRequestException)
            {
                return new(); // الصفحة تعرض رسالة "لا يوجد اتصال"
            }
        }


        // ================== Student Finance ==================

        public async Task<StudentFinanceViewLite?> GetStudentFinanceAsync(Guid studentId, Guid studentSemesterId)
        {
            try
            {
                var res = await _client
                    .From<StudentFinanceViewLite>()
                    .Where(x => x.StudentSemesterId == studentSemesterId && x.StudentId == studentId)
                    .Limit(1)
                    .Get();

                return res.Models.FirstOrDefault();
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }


        public async Task<List<PaymentLite>> GetStudentPaymentsAsync(Guid studentSemesterId)
        {
            try
            {
                var res = await _client
                    .From<PaymentLite>()
                    .Where(p => p.StudentSemesterId == studentSemesterId)
                    .Get();

                return res.Models
                    .OrderByDescending(p => p.PaidAt ?? p.CreatedAt)
                    .ToList();
            }
            catch (HttpRequestException)
            {
                return new();
            }
        }



    }
}

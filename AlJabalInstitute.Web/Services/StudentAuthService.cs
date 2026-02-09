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

        // ================== LOGIN ==================
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

        // ================== STUDENT SEMESTERS (CARDS) ==================
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
                return new();
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

        // =====================================================================
        // ================== NEW: STUDENT ACADEMIC (READ ONLY) =================
        // =====================================================================

        // ترجع كل سمسترات الطالب من الـ view (مرتبة الأحدث أولاً)
        public async Task<List<StudentAcademicViewLite>> GetStudentAcademicSemestersAsync(Guid studentId)
        {
            try
            {
                var res = await _client
                    .From<StudentAcademicViewLite>()
                    .Where(x => x.StudentId == studentId)
                    .Get();

                return res.Models
                    .OrderByDescending(x => x.StartDate)
                    .ToList();
            }
            catch (HttpRequestException)
            {
                return new();
            }
        }

        // منطق إظهار النتيجة (نفس منطق الإدارة)
        public bool IsAcademicResultVisible(
            bool isResultsPublished,
            bool isFinanciallyExempt,
            decimal remainingAmount,
            DateTime? resultUnlockedUntil)
        {
            if (!isResultsPublished)
                return false;

            if (isFinanciallyExempt || remainingAmount == 0m)
                return true;

            if (resultUnlockedUntil != null && resultUnlockedUntil > DateTime.UtcNow)
                return true;

            return false;
        }

        // تجميعة جاهزة للصفحة (Header + Cards)
        public async Task<StudentAcademicPageVM> GetStudentAcademicPageAsync(Guid studentId, Guid studentSemesterId)
        {
            var all = await GetStudentAcademicSemestersAsync(studentId);

            var header = all.FirstOrDefault(x => x.StudentSemesterId == studentSemesterId);

            var vm = new StudentAcademicPageVM
            {
                StudentName = header?.StudentName ?? "",
                Header = header,
                Semesters = new List<StudentAcademicSemesterCardVM>()
            };

            foreach (var x in all)
            {
                var published = x.IsResultsPublished ?? false;

                vm.Semesters.Add(new StudentAcademicSemesterCardVM
                {
                    StudentSemesterId = x.StudentSemesterId,
                    StudentId = x.StudentId,
                    SemesterId = x.SemesterId,
                    SemesterName = x.SemesterName ?? "",
                    StartDate = x.StartDate,

                    TotalSubjects = x.TotalSubjects ?? 0,
                    PassedSubjects = x.PassedSubjects ?? 0,
                    FailedSubjects = x.FailedSubjects ?? 0,
                    TotalScore = x.TotalScore ?? 0m,
                    Percentage = x.Percentage ?? 0m,
                    FinalResult = x.FinalResult ?? "—",

                    IsResultsPublished = published,
                    IsFinanciallyExempt = x.IsFinanciallyExempt,
                    RemainingAmount = x.RemainingAmount,
                    ResultUnlockedUntil = x.ResultUnlockedUntil,

                    IsVisible = IsAcademicResultVisible(
                        published,
                        x.IsFinanciallyExempt,
                        x.RemainingAmount,
                        x.ResultUnlockedUntil
                    )
                });
            }

            return vm;
        }
    }
}

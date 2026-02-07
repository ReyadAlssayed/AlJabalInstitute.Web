using AlJabalInstitute.Web.Models;
using Microsoft.Extensions.Configuration;
using Supabase;
using AlJabalInstitute.Web.Models; // يحتوي على Model Student
using System.Linq;
using System.Threading.Tasks;

namespace AlJabalInstitute.Web.Services
{
    public class StudentAuthService
    {
        private readonly IConfiguration _config;
        private Client? _client;

        public StudentAuthService(IConfiguration config)
        {
            _config = config;
        }

        // =========================
        // Init Supabase
        // =========================
        private async Task EnsureClientAsync()
        {
            if (_client != null)
                return;

            var url = _config["Supabase:Url"];
            var key = _config["Supabase:Key"];

            _client = new Client(url!, key!);
            await _client.InitializeAsync();
        }

        // =========================
        // LOGIN
        // =========================
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

            await EnsureClientAsync();

            // 🔍 جلب الطالب بالرقم الوطني
            var res = await _client!
                .From<Student>()
                .Where(s => s.NationalId == nationalId)
                .Limit(1)
                .Get();

            var student = res.Models.FirstOrDefault();

            // ❌ غير موجود أو غير مفعل
            if (student == null || !student.IsActive)
            {
                return new LoginResult
                {
                    Success = false,
                    Message = "بيانات الدخول غير صحيحة"
                };
            }

            // ❌ كلمة السر خاطئة
            if (string.IsNullOrWhiteSpace(student.Password) ||
                !BCrypt.Net.BCrypt.Verify(password, student.Password))
            {
                return new LoginResult
                {
                    Success = false,
                    Message = "بيانات الدخول غير صحيحة"
                };
            }

            // ✅ تحديث آخر تسجيل دخول
            student.LastLoginAt = DateTime.UtcNow;

            await _client!
                .From<Student>()
                .Where(s => s.Id == student.Id)
                .Set(s => s.LastLoginAt, student.LastLoginAt)
                .Update();

            // ✅ نجاح
            return new LoginResult
            {
                Success = true,
                StudentId = student.Id,
                StudentName = student.FullName
            };
        }
    }
}

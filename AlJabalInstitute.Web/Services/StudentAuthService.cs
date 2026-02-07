using AlJabalInstitute.Web.Models;
using Microsoft.Extensions.Configuration;
using Supabase;

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

        private async Task EnsureClientAsync()
        {
            if (_client != null) return;

            var url = _config["Supabase:Url"];
            var key = _config["Supabase:Key"];

            _client = new Client(url!, key!);
            await _client.InitializeAsync();
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

            await EnsureClientAsync();

            var res = await _client!
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

            await _client!
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
    }
}

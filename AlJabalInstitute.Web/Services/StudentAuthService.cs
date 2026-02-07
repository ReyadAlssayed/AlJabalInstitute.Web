using AlJabalInstitute.Web.Models;
using Microsoft.Extensions.Configuration;
using Supabase;
using System;
using System.Collections.Generic;
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

        // ✅ جديد: جلب فصول الطالب (مرتبة)
        public async Task<List<StudentSemesterCard>> GetStudentSemestersAsync(Guid studentId)
        {
            await EnsureClientAsync();

            var res = await _client!
                .From<StudentAcademicViewLite>()
                .Where(x => x.StudentId == studentId)
                .Get();

            return res.Models
                .Select(x => new StudentSemesterCard
                {
                    StudentSemesterId = x.StudentSemesterId,
                    StudentId = x.StudentId,
                    SemesterName = x.SemesterName ?? "",
                    StartDate = x.StartDate
                })
                .OrderByDescending(x => x.StartDate)
                .ToList();
        }
    }
}

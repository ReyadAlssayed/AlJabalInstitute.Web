using Microsoft.Extensions.Configuration;
using AlJabalInstitute.Web.Models;
using Supabase;

namespace AlJabalInstitute.Web.Services
{
    public class DataServices
    {
        private Client? _client;
        private readonly IConfiguration _config;

        public DataServices(IConfiguration config)
        {
            _config = config;
        }

        // =========================
        // Session (Current Teacher)
        // =========================
        public Teacher? CurrentTeacher { get; private set; }

        public bool IsLoggedIn => CurrentTeacher != null;

        public void OpenSession(Teacher teacher)
        {
            CurrentTeacher = teacher;
        }

        public void CloseSession()
        {
            CurrentTeacher = null;
        }

        public void Logout()
        {
            CurrentTeacher = null;
        }

        // =========================
        // Init Supabase (WEB)
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
        // Register - Check duplicate name
        // =========================
        public async Task<bool> TeacherNameExistsAsync(string fullName)
        {
            await EnsureClientAsync();

            var result = await _client!
                .From<Teacher>()
                .Where(t => t.FullName == fullName)
                .Get();

            return result.Models.Any();
        }

        // =========================
        // Register - Create new teacher
        // =========================
        public async Task<bool> CreateTeacherAsync(Teacher teacher)
        {
            await EnsureClientAsync();

            var response = await _client!
                .From<Teacher>()
                .Insert(teacher);

            return response.Models.Any();
        }

        // =========================
        // Login
        // =========================
        public async Task<Teacher?> LoginAsync(string fullName, string password)
        {
            await EnsureClientAsync();

            var result = await _client!
                .From<Teacher>()
                .Where(t => t.FullName == fullName && t.Password == password)
                .Get();

            var teacher = result.Models.FirstOrDefault();

            if (teacher != null)
                OpenSession(teacher);

            return teacher;
        }

        public async Task<bool> TeacherExistsAsync(string fullName)
        {
            await EnsureClientAsync();

            var result = await _client!
                .From<Teacher>()
                .Where(t => t.FullName == fullName)
                .Get();

            return result.Models.Any();
        }

        // =========================
        // Change Password
        // =========================
        public async Task<bool> ChangePasswordAsync(string currentPassword, string newPassword)
        {
            await EnsureClientAsync();

            if (CurrentTeacher == null)
                return false;

            var check = await _client!
                .From<Teacher>()
                .Where(t =>
                    t.Id == CurrentTeacher.Id &&
                    t.Password == currentPassword)
                .Get();

            if (!check.Models.Any())
                return false;

            CurrentTeacher.Password = newPassword;

            await _client!
                .From<Teacher>()
                .Update(CurrentTeacher);

            return true;
        }

        // =========================
        // Semesters
        // =========================
        public async Task<List<Semesters>> GetSemestersAsync()
        {
            await EnsureClientAsync();

            if (CurrentTeacher == null)
                return new List<Semesters>();

            var result = await _client!
                .From<Semesters>()
                .Where(s => s.TeacherId == CurrentTeacher.Id)
                .Order(s => s.StartDate, Supabase.Postgrest.Constants.Ordering.Ascending)
                .Get();

            return result.Models;
        }

        public async Task<bool> AddSemesterAsync(Semesters semester)
        {
            await EnsureClientAsync();

            var response = await _client!
                .From<Semesters>()
                .Insert(semester);

            return response.Models.Any();
        }

        public async Task<bool> UpdateSemesterAsync(Semesters semester)
        {
            await EnsureClientAsync();

            var response = await _client!
                .From<Semesters>()
                .Update(semester);

            return response.Models.Any();
        }

        public async Task<bool> DeleteSemesterAsync(Guid semesterId)
        {
            await EnsureClientAsync();

            await _client!
                .From<Semesters>()
                .Where(s => s.Id == semesterId)
                .Delete();

            return true;
        }

        // =========================
        // Subjects
        // =========================
        public async Task<List<Subjects>> GetAllSubjectsAsync()
        {
            await EnsureClientAsync();

            var result = await _client!
                .From<Subjects>()
                .Order(s => s.SubjectName, Supabase.Postgrest.Constants.Ordering.Ascending)
                .Get();

            return result.Models;
        }

        public async Task<List<Subjects>> GetSubjectsBySemesterAsync(Guid semesterId)
        {
            await EnsureClientAsync();

            var links = await _client!
                .From<semester_subject>()
                .Where(ss => ss.SemesterId == semesterId)
                .Get();

            if (!links.Models.Any())
                return new List<Subjects>();

            var subjectIds = links.Models.Select(ss => ss.SubjectId).ToList();

            var subjects = await _client!
                .From<Subjects>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.In, subjectIds)
                .Get();

            return subjects.Models;
        }

        public async Task<bool> AddSubjectToSemesterAsync(Guid semesterId, Guid subjectId)
        {
            await EnsureClientAsync();

            var exists = await _client!
                .From<semester_subject>()
                .Where(ss =>
                    ss.SemesterId == semesterId &&
                    ss.SubjectId == subjectId)
                .Get();

            if (exists.Models.Any())
                return false;

            var response = await _client!
                .From<semester_subject>()
                .Insert(new semester_subject
                {
                    SemesterId = semesterId,
                    SubjectId = subjectId
                });

            return response.Models.Any();
        }

        public async Task<bool> RemoveSubjectFromSemesterAsync(Guid semesterId, Guid subjectId)
        {
            await EnsureClientAsync();

            await _client!
                .From<semester_subject>()
                .Where(ss =>
                    ss.SemesterId == semesterId &&
                    ss.SubjectId == subjectId)
                .Delete();

            return true;
        }

        public async Task<Subjects?> CreateSubjectAsync(string subjectName)
        {
            try
            {
                await EnsureClientAsync();

                var normalizedName = subjectName.Trim();

                var existing = await _client!
                    .From<Subjects>()
                    .Where(s => s.SubjectName == normalizedName)
                    .Get();

                if (existing.Models.Any())
                    return existing.Models.First();

                var subject = new Subjects
                {
                    SubjectName = normalizedName
                };

                var response = await _client!
                    .From<Subjects>()
                    .Insert(subject);

                return response.Models.FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> UpdateSubjectAsync(Subjects subject)
        {
            await EnsureClientAsync();

            var response = await _client!
                .From<Subjects>()
                .Update(subject);

            return response.Models.Any();
        }
    }
}

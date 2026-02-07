namespace AlJabalInstitute.Web.Services
{
    public enum ToastType
    {
        Success,
        Error
    }

    public class ToastService
    {
        public event Action<string, int, ToastType>? OnShow;

        public void Success(string message, int ms = 2200)
            => OnShow?.Invoke(message, ms, ToastType.Success);

        public void Error(string message, int ms = 3200)
            => OnShow?.Invoke(message, ms, ToastType.Error);

        // اختياري: عام
        public void Show(string message, ToastType type, int ms = 2500)
            => OnShow?.Invoke(message, ms, type);
    }
}

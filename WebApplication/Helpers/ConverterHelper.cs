namespace WApp.Helpers
{
    public static class ConverterHelper
    {
        public static DateTime? StringToDateTime(string? value)
        {
            if(string.IsNullOrEmpty(value)) return null;
            return DateTime.ParseExact(value, "dd/MM/yyyy", null);
        }

        public static string? DateTimeToString(DateTime? value)
        {
            return value?.ToString("dd/MM/yyyy");
        }

    }
}

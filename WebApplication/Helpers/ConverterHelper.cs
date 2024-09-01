namespace WApp.Helpers
{
    public static class ConverterHelper
    {
        public static DateTime? StringToDateTime(string? value)
        {
            try
            {
                if (string.IsNullOrEmpty(value)) return null;
                if (value.Length > 10) return DateTime.Parse(value);
                return DateTime.ParseExact(value, "dd/MM/yyyy", null);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string? DateTimeToString(DateTime? value)
        {
            return value?.ToString("dd/MM/yyyy");
        }

    }
}

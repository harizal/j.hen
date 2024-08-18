using System.ComponentModel;

namespace WApp.Utlis
{
    public class Enums
    {
        public enum DTOrderDir
        {
            ASC,
            DESC
        }

        public enum NotificationType
        {
            error,
            success,
            warning,
            info
        }

        public enum PegawaiType
        {
            PHL,
            K2
        }

        public enum JenisKelamin
        {
            [Description("Laki-Laki")]
            L,
            [Description("Perempuan")]
            P
        }
    }
}

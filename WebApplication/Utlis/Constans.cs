namespace WApp.Utlis
{
    public class Constans
    {
        public const string ApplicationName = "SIPNONA (Sistem Informasi Pegawai Non ASN)";

        public const string RoleAdministrator = "Administrator";
        public const string RoleUser = "User";

        public const string NameUserDefault = "Administrator";
        public const string EmailUserDefault = "userAdmin@email.com";
        public const string PasswordUserDefault = ".4%H_M1K9[s.gmJ";

        public class Label
        {
            public const string InvalidLogin = "Invalid login attempt.";
            public const string AlreadyExists = "{0} already exists.";
            public const string DataNotFound = "Data not found.";
            public const string SavedSuccess = "Data has been saved successfully.";

            public const string CanntDeleteCurrentUser = "You are not allowed to delete the current user.";
            public const string CanntDeleteSuperAdmin = "You are not allowed to delete the Super Admin.";
        }
    }
}

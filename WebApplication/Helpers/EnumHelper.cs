using System.ComponentModel;
using System.Reflection;

namespace WApp.Helpers
{
    public static class EnumHelper
    {
        public static T GetEnumValueFromDescription<T>(string description)
        {
            MemberInfo[] fis = typeof(T).GetFields();

            foreach (var fi in fis)
            {
                DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attributes != null && attributes.Length > 0 && attributes[0].Description == description)
                    return (T)Enum.Parse(typeof(T), fi.Name);
            }

            throw new Exception("Not found");
        }

        public static string GetDescription(Enum en)
        {
            var type = en.GetType();

            var memInfo = type.GetMember(en.ToString());

            if (memInfo.Length <= 0)
                return en.ToString();

            var attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attrs.Length > 0 ? ((DescriptionAttribute)attrs[0]).Description : en.ToString();
        }
    }
}

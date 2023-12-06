using XFE各类拓展.NetCore.FileExtension;
using XFE各类拓展.NetCore.FormatExtension;

namespace XFECommunity.AllImpl
{
    public enum LoginMethod
    {
        PasswordLogin,
        VerifyCodeLogin
    }
    public class AppSystemProfile
    {
        public static LoginMethod LoginMethod { get; set; } = LoginMethod.PasswordLogin;
        public static string IgnoreVersion { get; set; } = string.Empty;
        public static AppTheme Theme { get; set; } = Application.Current is null ? AppTheme.Unspecified : Application.Current.RequestedTheme;
        public static void SaveSystemProfile()
        {
            string saveString = new XFEDictionary(new string[]
            {
                nameof(LoginMethod), LoginMethod.ToString(),
                nameof(IgnoreVersion), IgnoreVersion,
                nameof(Theme), Theme.ToString()
            }).ToString();
            saveString.WriteIn(AppPath.SystemProfilePath);
        }
        public static void LoadSystemProfile()
        {
            if (AppPath.SystemProfilePath.ReadOut(out var profileString))
            {
                if (profileString is not null)
                {
                    var profileProperty = new XFEDictionary(profileString);
                    foreach (var property in profileProperty)
                    {
                        switch (property.Header)
                        {
                            case nameof(LoginMethod):
                                LoginMethod = Enum.Parse<LoginMethod>(property.Content);
                                break;
                            case nameof(IgnoreVersion):
                                IgnoreVersion = property.Content;
                                break;
                            case nameof(Theme):
                                Theme = Enum.Parse<AppTheme>(property.Content);
                                break;
                        }
                    }
                }
            }
        }
    }
}

﻿using XFE各类拓展.NetCore.FileExtension;
using XFE各类拓展.NetCore.FormatExtension;

namespace XCCChatRoom.AllImpl
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
        public static void SaveSystemProfile()
        {
            string saveString = new XFEDictionary(new string[]
            {
                nameof(LoginMethod), LoginMethod.ToString(),
                nameof(IgnoreVersion), IgnoreVersion
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
                                switch (property.Content)
                                {
                                    case "PasswordLogin":
                                        LoginMethod = LoginMethod.PasswordLogin;
                                        break;
                                    case "VerifyCodeLogin":
                                        LoginMethod = LoginMethod.VerifyCodeLogin;
                                        break;
                                    default:
                                        try { Shell.Current?.DisplayAlert("错误", $"配置文件读取错误，未知的类型：{property.Content}", "我测"); } catch { }
                                        break;
                                }
                                break;
                            case nameof(IgnoreVersion):
                                IgnoreVersion = property.Content;
                                break;
                        }
                    }
                }
            }
        }
    }
}

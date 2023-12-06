﻿using XFECommunity.AllImpl;
using XFECommunity.Resources.Styles;

namespace XFECommunity
{
    public partial class App : Application
    {
        public App()
        {
            Current!.RequestedThemeChanged += (s, a) =>
            {
                if (Current!.UserAppTheme != AppTheme.Unspecified)
                    AppSystemProfile.Theme = a.RequestedTheme;
                AutoSwitchByTheme(a.RequestedTheme);
            };
            InitializeComponent();
            AppSystemProfile.Theme = Current!.RequestedTheme;
            Console.WriteLine(Current!.RequestedTheme);
            AppSystemProfile.LoadSystemProfile();
            MainPage = new AppShell();
            AutoSwitchByTheme(AppSystemProfile.Theme);
        }
        public static void SwitchToTheme(ResourceDictionary resourceDictionary)
        {
            if (Current is not null)
            {
                Current.Resources.MergedDictionaries.Clear();
                Current.Resources.MergedDictionaries.Add(resourceDictionary);
            }
        }
        public static void AutoSwitchByTheme(AppTheme appTheme)
        {
            switch (appTheme)
            {
                case AppTheme.Unspecified:
                    switch (Current!.RequestedTheme)
                    {
                        case AppTheme.Unspecified:
                            SwitchToTheme(new LightTheme());
                            break;
                        case AppTheme.Light:
                            SwitchToTheme(new LightTheme());
                            break;
                        case AppTheme.Dark:
                            SwitchToTheme(new DarkTheme());
                            break;
                        default:
                            break;
                    }
                    break;
                case AppTheme.Light:
                    SwitchToTheme(new LightTheme());
                    break;
                case AppTheme.Dark:
                    SwitchToTheme(new DarkTheme());
                    break;
                default:
                    break;
            }
        }
    }
}

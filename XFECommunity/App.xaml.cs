using XFECommunity.Resources.Styles;

namespace XFECommunity
{
    public partial class App : Application
    {
        public App()
        {
            Current!.RequestedThemeChanged += (s, a) =>
            {
                AutoSwitchByTheme(a.RequestedTheme);
            };
            InitializeComponent();
            MainPage = new AppShell();
            AutoSwitchByTheme(Current!.RequestedTheme);
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
        }
    }
}

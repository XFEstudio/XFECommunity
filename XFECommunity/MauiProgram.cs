using Microsoft.Extensions.Logging;
using XFECommunity.AllImpl;

namespace XFECommunity
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            XCCDataBase.Initialize();
            TencentSms.Initialize();
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .ConfigureMauiHandlers(handlers =>
                {
#if WINDOWS
                    handlers.AddHandler(typeof(Entry), typeof(Platforms.Windows.WinEntryHandler));
                    handlers.AddHandler(typeof(Editor), typeof(Platforms.Windows.WinEditorHandler));
#endif
                });
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}

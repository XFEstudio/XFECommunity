using Microsoft.Extensions.Logging;
using XCCChatRoom.AllImpl;

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
                });
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}

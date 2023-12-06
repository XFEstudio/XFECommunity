using Microsoft.UI.Xaml.Controls;

namespace XFECommunity.Platforms.Windows
{
    public class WinEntryHandler : Microsoft.Maui.Handlers.EntryHandler
    {
        protected override void ConnectHandler(TextBox platformView)
        {
            platformView.Background = null;
            base.ConnectHandler(platformView);
        }
    }
    public class WinEditorHandler : Microsoft.Maui.Handlers.EditorHandler
    {
        protected override void ConnectHandler(TextBox platformView)
        {
            platformView.Background = null;
            base.ConnectHandler(platformView);
        }
    }
}

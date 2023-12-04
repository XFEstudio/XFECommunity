using XFECommunity.ViewPage;

namespace XFECommunity
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            this.BindingContext = this;
            Routing.RegisterRoute(nameof(UserLoginPage), typeof(UserLoginPage));
            Routing.RegisterRoute(nameof(UserRegisterPage), typeof(UserRegisterPage));
            Routing.RegisterRoute(nameof(PostEditPage), typeof(PostEditPage));
            Routing.RegisterRoute(nameof(PostViewPage), typeof(PostViewPage));
        }
    }
}

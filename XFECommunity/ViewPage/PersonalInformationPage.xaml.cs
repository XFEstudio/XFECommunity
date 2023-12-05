
namespace XFECommunity.ViewPage;

public partial class PersonalInformationPage : ContentPage
{
    public string CurrentUserNameLabelText
    {
        get
        { return CurrentUserNameLabel.Text; }
        set
        { CurrentUserNameLabel.Dispatcher.Dispatch(() => CurrentUserNameLabel.Text = value); }
    }

    public string CurrentTelLabelText
    {
        get { return CurrentTelLabel.Text; }
        set { CurrentTelLabel.Dispatcher.Dispatch(() => CurrentTelLabel.Text = value); }
    }

    public string CurrentEmailLabelText
    {
        get { return CurrentMailLabel.Text; }
        set { CurrentMailLabel.Dispatcher.Dispatch(() => CurrentMailLabel.Text = value); }
    }

    public string UserUUIDLabelText
    {
        get { return UserUUIDLabel.Text; }
        set { UserUUIDLabel.Dispatcher.Dispatch(() => UserUUIDLabel.Text = value); }
    }

    public PersonalInformationPage()
    {
        InitializeComponent();
        if (UserInfo.IsLoginSuccessful && UserInfo.CurrentUser is not null)
        {
            CurrentUserNameLabelText = UserInfo.CurrentUser.Aname!;
            CurrentTelLabelText = UserInfo.CurrentUser.Atel!;
            CurrentEmailLabelText = UserInfo.CurrentUser.Amail!;
            UserUUIDLabelText = UserInfo.CurrentUser.ID!;

        }
        
        
    }

    private async void UserNameEdit_Tapped(object sender, TappedEventArgs e)
    {
        string userNameEdit = await DisplayPromptAsync("修改昵称", "请输入要修改的昵称", "确定","取消");
        if (userNameEdit is not null && userNameEdit != string.Empty)
        {
            if (false)
            {
                /*UserInfo.*/
                await DisplayAlert("修改成功", "内容合法", "明白了");
            }
            else
            {
                await DisplayAlert("非法昵称", "请输入合法昵称", "明白了");
            }
        }
    }

    private void UserTelEdit_Clicked(object sender, EventArgs e)
    {

    }

    private void UserMailEdit_Clicked(object sender, EventArgs e)
    {

    }

    private void UserPasswordEdit_Clicked(object sender, EventArgs e)
    {

    }

}
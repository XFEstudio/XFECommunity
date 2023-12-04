using XCCChatRoom.AllImpl;

namespace XFECommunity.ViewPage;

public partial class PersonalInformationPage : ContentPage
{
    public string CurrentUserNameLabelText
    {
        get
        {
            return CurrentUserNameLabel.Text;
        }
        set
        {
            CurrentUserNameLabel.Dispatcher.Dispatch(() => CurrentUserNameLabel.Text = value);
        }
    }

    public PersonalInformationPage()
    {
        InitializeComponent();
        /*CurrentUserNameLabelText = */

    }

    private async void UserNameEdit_Tapped(object sender, TappedEventArgs e)
    {
        string userNameEdit = await DisplayPromptAsync("修改昵称", "请输入要修改的昵称", "确定","取消");
        if (userNameEdit is not null && userNameEdit != string.Empty)
        {
            if (userNameEdit.UserNameEditor())
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

    private void GetFavoritePosts_Tapped(object sender, TappedEventArgs e)
    {

    }
}
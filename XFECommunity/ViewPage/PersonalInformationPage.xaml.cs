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
        string userNameEdit = await DisplayPromptAsync("�޸��ǳ�", "������Ҫ�޸ĵ��ǳ�", "ȷ��","ȡ��");
        if (userNameEdit is not null && userNameEdit != string.Empty)
        {
            if (userNameEdit.UserNameEditor())
            {
                /*UserInfo.*/
                await DisplayAlert("�޸ĳɹ�", "���ݺϷ�", "������");
            }
            else
            {
                await DisplayAlert("�Ƿ��ǳ�", "������Ϸ��ǳ�", "������");
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
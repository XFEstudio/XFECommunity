
using XFECommunity.AllImpl;
using XFE������չ.NetCore.StringExtension;
using XFE������չ.NetCore.XFEDataBase;

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

    private async void UserTelEdit_Clicked(object sender, EventArgs e)
    {
        string userTelEdit = await DisplayPromptAsync("�޸��ֻ���", "��������Ҫ�޸ĵ��ֻ���", "ȷ��", "ȡ��");
        if (userTelEdit is not null && userTelEdit != string.Empty)
        {
            if (userTelEdit.IsMobPhoneNumber())
            {
                UserInfo.EditUserProperty(UserPropertyToEdit.PhoneNum, userTelEdit);
                await DisplayAlert("�޸ĳɹ�", "�����ֻ������޸�", "������");
            }
            else
            {
                await DisplayAlert("�Ƿ��ֻ���", "������Ϸ��ֻ���", "������");
            }
        }
    }

    private async void UserMailEdit_Clicked(object sender, EventArgs e)
    {
        string userMailEdit = await DisplayPromptAsync("�޸�����", "��������Ҫ�޸ĵ�����", "ȷ��", "ȡ��");
        if (userMailEdit is not null && userMailEdit != string.Empty)
        {
            if (userMailEdit.IsValidEmail())
            {
                UserInfo.EditUserProperty(UserPropertyToEdit.Mail, userMailEdit);
                await DisplayAlert("�޸ĳɹ�", "�����������޸�", "������");
            }
            else
            {
                await DisplayAlert("���䲻����", "�������������", "������");
            }
        }
    }

    private async void UserPasswordEdit_Clicked(object sender, EventArgs e)
    {
        string userPasswordEdit = await DisplayPromptAsync("�޸�����", "��������Ҫ�޸ĵ�����", "ȷ��", "ȡ��");
        if (userPasswordEdit is not null && userPasswordEdit != string.Empty) 
        {
            if(userPasswordEdit.PasswordEditor())
            {
                UserInfo.EditUserProperty(UserPropertyToEdit.Password, userPasswordEdit);
                await DisplayAlert("�޸ĳɹ�", "�����������޸�", "������");
            }
            else
            {
                await DisplayAlert("�Ƿ�����", "������Ϸ�����", "������");
            }
        }
    }

    private async void UserNameEdit_Clicked(object sender, EventArgs e)
    {
        string userNameEdit = await DisplayPromptAsync("�޸��ǳ�", "��������Ҫ�޸ĵ��ǳ�", "ȷ��", "ȡ��");
        if (userNameEdit is not null && userNameEdit != string.Empty)
        {
            if (userNameEdit.UserNameEditor())
            {
                UserInfo.EditUserProperty(UserPropertyToEdit.UserName, userNameEdit);
                await DisplayAlert("�޸ĳɹ�", "�����������޸�", "������");
            }
            else
            {
                await DisplayAlert("�Ƿ��ǳ�", "������Ϸ��ǳ�", "������");
            }
        }
    }
    public async void ForgetPassword()
    {
        string userTel = await DisplayPromptAsync("��������", "�����������ֻ���", "ȷ��", "ȡ��");
        if (userTel is not null && userTel != string.Empty)
        {
            var xFEExecuter = XCCDataBase.XFEDataBase.CreateExecuter();
            var result = await xFEExecuter.ExecuteGet<XFEChatRoom_UserInfoForm>(x => x.Atel == userTel);
            if (result is not null && result.Count == 1)
            {
                ChangeForgetPassword();
            }
            elsev
            {
                await DisplayAlert("���쳣", "���ֻ���δע�����˺����쳣", "������");
            }
        }
    }

    public async void ChangeForgetPassword()
    {
        string userPassword = await DisplayPromptAsync("��������", "����������������", "ȷ��");
        if (userPassword is not null && userPassword.PasswordEditor())
        {
            UserInfo.EditUserProperty(UserPropertyToEdit.Password, userPassword, this);
            if (await UserInfo.UpLoadUserInfo() == 1)
            {
                await DisplayAlert("�޸ĳɹ�", "�����������޸�", "������");
            }
            else { await DisplayAlert("�޸�ʧ��", "���������޸��쳣", "������"); }
        }
        else { await DisplayAlert("�����쳣", "����������벻�Ϲ�", "������"); }
    }
}

using XFECommunity.AllImpl;
using XFE各类拓展.NetCore.StringExtension;
using XFE各类拓展.NetCore.XFEDataBase;

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
        string userTelEdit = await DisplayPromptAsync("修改手机号", "请输入您要修改的手机号", "确定", "取消");
        if (userTelEdit is not null && userTelEdit != string.Empty)
        {
            if (userTelEdit.IsMobPhoneNumber())
            {
                UserInfo.EditUserProperty(UserPropertyToEdit.PhoneNum, userTelEdit);
                await DisplayAlert("修改成功", "您的手机号已修改", "明白了");
            }
            else
            {
                await DisplayAlert("非法手机号", "请输入合法手机号", "明白了");
            }
        }
    }

    private async void UserMailEdit_Clicked(object sender, EventArgs e)
    {
        string userMailEdit = await DisplayPromptAsync("修改邮箱", "请输入您要修改的邮箱", "确定", "取消");
        if (userMailEdit is not null && userMailEdit != string.Empty)
        {
            if (userMailEdit.IsValidEmail())
            {
                UserInfo.EditUserProperty(UserPropertyToEdit.Mail, userMailEdit);
                await DisplayAlert("修改成功", "您的邮箱已修改", "明白了");
            }
            else
            {
                await DisplayAlert("邮箱不合理", "请输入合理邮箱", "明白了");
            }
        }
    }

    private async void UserPasswordEdit_Clicked(object sender, EventArgs e)
    {
        string userPasswordEdit = await DisplayPromptAsync("修改密码", "请输入您要修改的密码", "确定", "取消");
        if (userPasswordEdit is not null && userPasswordEdit != string.Empty) 
        {
            if(userPasswordEdit.PasswordEditor())
            {
                UserInfo.EditUserProperty(UserPropertyToEdit.Password, userPasswordEdit);
                await DisplayAlert("修改成功", "您的密码已修改", "明白了");
            }
            else
            {
                await DisplayAlert("非法密码", "请输入合法密码", "明白了");
            }
        }
    }

    private async void UserNameEdit_Clicked(object sender, EventArgs e)
    {
        string userNameEdit = await DisplayPromptAsync("修改昵称", "请您输入要修改的昵称", "确定", "取消");
        if (userNameEdit is not null && userNameEdit != string.Empty)
        {
            if (userNameEdit.UserNameEditor())
            {
                UserInfo.EditUserProperty(UserPropertyToEdit.UserName, userNameEdit);
                await DisplayAlert("修改成功", "您的密码已修改", "明白了");
            }
            else
            {
                await DisplayAlert("非法昵称", "请输入合法昵称", "明白了");
            }
        }
    }
    public async void ForgetPassword()
    {
        string userTel = await DisplayPromptAsync("忘记密码", "请输入您的手机号", "确定", "取消");
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
                await DisplayAlert("绑定异常", "该手机号未注册或绑定账号数异常", "明白了");
            }
        }
    }

    public async void ChangeForgetPassword()
    {
        string userPassword = await DisplayPromptAsync("忘记密码", "请输入您的新密码", "确定");
        if (userPassword is not null && userPassword.PasswordEditor())
        {
            UserInfo.EditUserProperty(UserPropertyToEdit.Password, userPassword, this);
            if (await UserInfo.UpLoadUserInfo() == 1)
            {
                await DisplayAlert("修改成功", "您的密码已修改", "明白了");
            }
            else { await DisplayAlert("修改失败", "您的密码修改异常", "明白了"); }
        }
        else { await DisplayAlert("输入异常", "您输入的密码不合规", "明白了"); }
    }
}
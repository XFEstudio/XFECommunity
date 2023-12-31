using XFECommunity.AllImpl;
using XFECommunity.Controls;
using XFE各类拓展.NetCore.FileExtension;
using XFE各类拓展.NetCore.FormatExtension;
using XFE各类拓展.NetCore.StringExtension;
using XFE各类拓展.NetCore.XFEDataBase;

namespace XFECommunity.ViewPage;
public enum UserPropertyToEdit
{
    UserName,
    Mail,
    Password,
    PhoneNum
}
public partial class UserInfo : ContentPage
{
    public static readonly BindableProperty UserNameProperty = BindableProperty.Create(nameof(UserName), typeof(string), typeof(UserInfo), string.Empty);
    public string UserName
    {
        get => (string)GetValue(UserNameProperty);
        set
        {
            SetValue(UserNameProperty, value);
        }
    }
    public string uUID = string.Empty;
    public string UUID
    {
        get => uUID;
        set
        {
            uuidLabel.Text = "UID：" + value;
            uUID = value;
        }
    }
    public static bool IsLoginSuccessful { get; set; } = false;
    public static string? StaticUUID { get; set; }
    public static string? StaticUserName { get; set; }
    public static string? StaticMail { get; set; }
    public static string? StaticPassword { get; set; }
    public static string? StaticPhoneNum { get; set; }
    private static XFEChatRoom_UserInfoForm? currentUser;
    public static XFEChatRoom_UserInfoForm? CurrentUser
    {
        get
        {
            if (currentUser is null)
                return null;
            currentUser.Agroups ??= string.Empty;
            currentUser.LikedPostID ??= string.Empty;
            currentUser.LikedCommentID ??= string.Empty;
            currentUser.LatestMessage ??= string.Empty;
            currentUser.StarredPostID ??= string.Empty;
            return currentUser;
        }
        set
        {
            currentUser = value;
        }
    }
    public static UserInfo? CurrentPage { get; private set; }
    private static readonly XFEExecuter XFEExecuter = XCCDataBase.XFEDataBase!.CreateExecuter();
    public UserInfo()
    {
        InitializeComponent();
        this.BindingContext = this;
        SwitchToTheme(AppSystemProfile.Theme);
        CurrentPage = this;
        if (IsLoginSuccessful)
        {
            UserName = StaticUserName!;
            UUID = StaticUUID!;
            SwitchToLoginStyle();
        }
        else
        {
            charLabel.Text = "?";
            nameLabel.Text = "未登录";
            uuidLabel.Text = "暂无UID";
            StaticUserName = string.Empty;
            StaticUUID = string.Empty;
            StaticPassword = string.Empty;
            StaticPhoneNum = string.Empty;
        }
    }
    public async static Task<int> UpLoadUserInfo()
    {
        try
        {
            return await CurrentUser!.ExecuteUpdate(XFEExecuter);
        }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync(ex.ToString());
            return 0;
        }
    }

    public void SwitchToTheme(AppTheme appTheme)
    {
        switch (appTheme)
        {
            case AppTheme.Unspecified:
                ThemeShowImage.Source = "sunandmoon.png";
                ThemeShowLabel.Text = "跟随系统";
                break;
            case AppTheme.Light:
                ThemeShowImage.Source = "sun.png";
                ThemeShowLabel.Text = "浅色";
                break;
            case AppTheme.Dark:
                ThemeShowImage.Source = "moon.png";
                ThemeShowLabel.Text = "深色";
                break;
            default:
                break;
        }
    }

    public void SwitchToLoginStyle()
    {
        LoginButton.SetDynamicResource(Button.BackgroundColorProperty, "WhiteInLightAndTransparentInDarkColor");
        LoginButton.Text = "退出登录";
        LoginButton.BorderColor = Color.Parse("Red");
        LoginButton.BorderWidth = 1;
        LoginButton.TextColor = Color.Parse("Red");
        LoginButton.WaitClick -= LoginButton_WaitClick;
        LoginButton.WaitClick += UnLoginButton_WaitClick;
    }
    public void SwitchToUnLoginStyle()
    {
        IsLoginSuccessful = false;
        charLabel.Text = "?";
        nameLabel.Text = "未登录";
        uuidLabel.Text = "暂无UID";
        StaticUserName = string.Empty;
        StaticUUID = string.Empty;
        StaticPassword = string.Empty;
        StaticPhoneNum = string.Empty;
        LoginButton.SetDynamicResource(Button.BackgroundColorProperty, "MainColor");
        LoginButton.Text = "登录";
        LoginButton.BorderColor = null;
        LoginButton.BorderWidth = 0;
        LoginButton.SetDynamicResource(Button.TextColorProperty, "BackGroundColor");
        CommunityPage.Current?.ChangeToUnLoginStyle();
        try
        {
            File.Delete(AppPath.UserInfoPath);
        }
        catch (Exception ex)
        {
            DisplayAlert("错误", ex.Message, "确认");
        }
    }
    public static void SaveUserData(Page CurrentPage)
    {
        try
        {
            string Locality = new XFEDictionary(new string[]
            {
                "UUID", StaticUUID!,
                "UserName", StaticUserName!,
                "Mail", StaticMail!,
                "Password", StaticPassword!,
                "PhoneNum", StaticPhoneNum!
            }).ToString();
            Locality.WriteIn(AppPath.UserInfoPath);
        }
        catch (Exception ex)
        {
            CurrentPage.DisplayAlert("错误", ex.Message, "确认");
        }
    }
    public static async void EditUserProperty(UserPropertyToEdit userPropertyToEdit, string newProperty)
    {
        switch (userPropertyToEdit)
        {
            case UserPropertyToEdit.UserName:
                StaticUserName = newProperty;
                CurrentPage!.UserName = newProperty;
                CurrentUser!.Aname = newProperty;
                break;

            case UserPropertyToEdit.Password:
                StaticPassword = newProperty;
                CurrentUser!.Apassword = newProperty;
                break;

            case UserPropertyToEdit.PhoneNum:
                StaticMail = newProperty;
                CurrentUser!.Atel = newProperty;
                break;

            case UserPropertyToEdit.Mail:
                StaticMail = newProperty;
                CurrentUser!.Amail = newProperty;
                break;
            default:
                await ProcessException.ShowEnumException();
                break;
        }
        await CurrentUser!.ExecuteUpdate(XFEExecuter);
    }
    public static async Task ReadUserData(Page CurrentPage)
    {
        try
        {
            if (AppPath.UserInfoPath.ReadOut(out var userInfo))
            {
                if (userInfo is null)
                    return;
                var userProperties = new XFEDictionary(userInfo);
                if (userProperties is null || userProperties.Count == 0)
                {
                    IsLoginSuccessful = false;
                }
                else
                {
                    foreach (var property in userProperties)
                    {
                        try
                        {
                            switch (property.Header)
                            {
                                case "UserName":
                                    StaticUserName = property.Content;
                                    break;
                                case "UUID":
                                    StaticUUID = property.Content;
                                    break;
                                case "Password":
                                    StaticPassword = property.Content;
                                    break;
                                case "PhoneNum":
                                    StaticPhoneNum = property.Content;
                                    break;
                                case "Mail":
                                    StaticMail = property.Content;
                                    break;
                                default:
                                    await ProcessException.ShowEnumException();
                                    File.Delete(AppPath.UserInfoPath);
                                    return;
                            }
                        }
                        catch (Exception)
                        {
                            await CurrentPage.DisplayAlert("配置文件错误", "读取用户文件时发生错误\n用户配置文件损坏，请重新登录", "确认");
                            File.Delete(AppPath.UserInfoPath);
                            return;
                        }
                    }
                    var user = await XFEExecuter.ExecuteGet<XFEChatRoom_UserInfoForm>(x => x.Atel == StaticPhoneNum);
                    if (user is null || user.Count == 0 || user.First() is null)
                    {
                        await CurrentPage.DisplayAlert("登录", "用户信息错误，请重新登录", "确认");
                        File.Delete(AppPath.UserInfoPath);
                        return;
                    }
                    else
                    {
                        if (user.First().Apassword != StaticPassword)
                        {
                            await CurrentPage.DisplayAlert("登录", "用户密码错误\n密码可能已被修改，账号或存在风险\n请重新登录", "确认");
                            File.Delete(AppPath.UserInfoPath);
                            return;
                        }
                        else
                        {
                            CurrentUser = user.First();
                            StaticMail = CurrentUser.Amail;
                            StaticUserName = CurrentUser.Aname;
                            StaticUUID = CurrentUser.ID;
                            StaticPhoneNum = CurrentUser.Atel;
                            StaticPassword = CurrentUser.Apassword;
                        }
                    }
                    IsLoginSuccessful = true;
                }
            }
        }
        catch (Exception ex)
        {
            await CurrentPage.DisplayAlert("登录错误", ex.ToString(), "确认");
        }
    }
    private async void LoginButton_WaitClick(object? sender, WaitButtonClickedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(UserLoginPage));
        e.Continue();
    }
    private async void UnLoginButton_WaitClick(object? sender, WaitButtonClickedEventArgs e)
    {
        if (await DisplayAlert("退出登录", "确定退出登录吗？", "确认", "取消"))
        {
            SwitchToUnLoginStyle();
            LoginButton.WaitClick -= UnLoginButton_WaitClick;
            LoginButton.WaitClick += LoginButton_WaitClick;
        }
        e.Continue();
    }

    private void SwitchThemeGesture_Tapped(object sender, TappedEventArgs e)
    {
        CommunityPage.SwitchToNextTheme();
    }

    private async void NameEditGesture_Tapped(object sender, TappedEventArgs e)
    {
        if (IsLoginSuccessful)
        {
            string userNameEdit = await DisplayPromptAsync("修改昵称", "请您输入要修改的昵称", "确定", "取消");
            if (userNameEdit is not null && userNameEdit != string.Empty)
            {
                if (userNameEdit.VerifyUserName())
                {
                    EditUserProperty(UserPropertyToEdit.UserName, userNameEdit);
                    await DisplayAlert("修改成功", "您的用户名已修改", "明白了");
                }
                else
                {
                    await DisplayAlert("非法昵称", "请输入合法昵称", "明白了");
                }
            }
        }
        else
        {
            await DisplayAlert("未登录", "请先登录", "确认");
        }
    }
    private async void PasswordEditGesture_Tapped(object sender, TappedEventArgs e)
    {
        if (IsLoginSuccessful)
        {
            string userPasswordEdit = await DisplayPromptAsync("修改密码", "请输入您要修改的密码", "确定", "取消");
            if (userPasswordEdit is not null && userPasswordEdit != string.Empty)
            {
                if (userPasswordEdit.VerifyPassword())
                {
                    EditUserProperty(UserPropertyToEdit.Password, userPasswordEdit);
                    await DisplayAlert("修改成功", "您的密码已修改", "明白了");
                }
                else
                {
                    await DisplayAlert("非法密码", "请输入合法密码", "明白了");
                }
            }
        }
        else
        {
            await DisplayAlert("未登录", "请先登录", "确认");
        }
    }

    private async void MailEditGesture_Tapped(object sender, TappedEventArgs e)
    {
        if (IsLoginSuccessful)
        {
            string userMailEdit = await DisplayPromptAsync("修改邮箱", "请输入您要修改的邮箱", "确定", "取消");
            if (userMailEdit is not null && userMailEdit != string.Empty)
            {
                if (userMailEdit.IsValidEmail())
                {
                    EditUserProperty(UserPropertyToEdit.Mail, userMailEdit);
                    await DisplayAlert("修改成功", "您的邮箱已修改", "明白了");
                }
                else
                {
                    await DisplayAlert("邮箱不合理", "请输入合理邮箱", "明白了");
                }
            }
        }
        else
        {
            await DisplayAlert("未登录", "请先登录", "确认");
        }
    }

    private async void TelEditGesture_Tapped(object sender, TappedEventArgs e)
    {
        if (IsLoginSuccessful)
        {
            string userTelEdit = await DisplayPromptAsync("修改手机号", "请输入您要修改的手机号", "确定", "取消");
            if (userTelEdit is not null && userTelEdit != string.Empty)
            {
                if (userTelEdit.IsMobPhoneNumber())
                {
                    EditUserProperty(UserPropertyToEdit.PhoneNum, userTelEdit);
                    await DisplayAlert("修改成功", "您的手机号已修改", "明白了");
                }
                else
                {
                    await DisplayAlert("非法手机号", "请输入合法手机号", "明白了");
                }
            }
        }
        else
        {
            await DisplayAlert("未登录", "请先登录", "确认");
        }
    }
}
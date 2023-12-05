using XFECommunity.AllImpl;
using XFECommunity.Controls;
using XFE������չ.NetCore.FileExtension;
using XFE������չ.NetCore.FormatExtension;
using XFE������չ.NetCore.XFEDataBase;

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
            uuidLabel.Text = "UID��" + value;
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
    private static XFEExecuter XFEExecuter = XCCDataBase.XFEDataBase!.CreateExecuter();
    public UserInfo()
    {
        InitializeComponent();
        this.BindingContext = this;
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
            nameLabel.Text = "δ��¼";
            uuidLabel.Text = "����UID";
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
    public void SwitchToLoginStyle()
    {
        LoginButton.SetDynamicResource(Button.BackgroundColorProperty, "WhiteInLightAndTransparentInDarkColor");
        LoginButton.Text = "�˳���¼";
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
        nameLabel.Text = "δ��¼";
        uuidLabel.Text = "����UID";
        StaticUserName = string.Empty;
        StaticUUID = string.Empty;
        StaticPassword = string.Empty;
        StaticPhoneNum = string.Empty;
        LoginButton.SetDynamicResource(Button.BackgroundColorProperty, "MainColor");
        LoginButton.Text = "��¼";
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
            DisplayAlert("����", ex.Message, "ȷ��");
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
            CurrentPage.DisplayAlert("����", ex.Message, "ȷ��");
        }
    }
    public static async void EditUserProperty(UserPropertyToEdit userPropertyToEdit, string newProperty, Page page)
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
                            await CurrentPage.DisplayAlert("�����ļ�����", "��ȡ�û��ļ�ʱ��������\n�û������ļ��𻵣������µ�¼", "ȷ��");
                            File.Delete(AppPath.UserInfoPath);
                            return;
                        }
                    }
                    var user = await XFEExecuter.ExecuteGet<XFEChatRoom_UserInfoForm>(x => x.Atel == StaticPhoneNum);
                    if (user is null || user.Count == 0 || user.First() is null)
                    {
                        await CurrentPage.DisplayAlert("��¼", "�û���Ϣ���������µ�¼", "ȷ��");
                        File.Delete(AppPath.UserInfoPath);
                        return;
                    }
                    else
                    {
                        if (user.First().Apassword != StaticPassword)
                        {
                            await CurrentPage.DisplayAlert("��¼", "�û��������\n��������ѱ��޸ģ��˺Ż���ڷ���\n�����µ�¼", "ȷ��");
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
            await CurrentPage.DisplayAlert("��¼����", ex.ToString(), "ȷ��");
        }
    }
    private async void LoginButton_WaitClick(object? sender, WaitButtonClickedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(UserLoginPage));
        e.Continue();
    }
    private async void UnLoginButton_WaitClick(object? sender, WaitButtonClickedEventArgs e)
    {
        if (await DisplayAlert("�˳���¼", "ȷ���˳���¼��", "ȷ��", "ȡ��"))
        {
            SwitchToUnLoginStyle();
            LoginButton.WaitClick -= UnLoginButton_WaitClick;
            LoginButton.WaitClick += LoginButton_WaitClick;
        }
        e.Continue();
    }
    //private void WhiteChoiceButton_Click(object sender, TappedEventArgs e)
    //{
    //    Shell.Current.GoToAsync(nameof(UserPrivacyListPage));
    //}

    //private void WhiteChoiceUserPropertyEditorButton_Click(object sender, TappedEventArgs e)
    //{
    //    if (IsLoginSuccessful)
    //        Shell.Current.GoToAsync(nameof(UserPropertyEditPage));
    //    else
    //        DisplayAlert("���ȵ�¼"));
    //}
}
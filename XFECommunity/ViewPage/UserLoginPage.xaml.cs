using XFECommunity.AllImpl;
using XFECommunity.Controls;
using XFE各类拓展.NetCore.StringExtension;
using XFE各类拓展.NetCore.TaskExtension;
using XFE各类拓展.NetCore.XFEDataBase;

namespace XFECommunity.ViewPage;

public partial class UserLoginPage : ContentPage
{
    private readonly XFEExecuter XFEExecuter = XCCDataBase.XFEDataBase!.CreateExecuter();
    private bool isAccountChanged = false, isPasswordChanged = false, isTelChanged = false;
    private bool isPasswordEditorEmpty = true, isAccountEditorEmpty = true, isTelEditorEmpty = true;
    private bool isCoolDown = false;
    private string currentPhoneNum = string.Empty;
    private string randomCode = string.Empty;
    public UserLoginPage()
    {
        try
        {
            InitializeComponent();
            new Action(async () =>
            {
                if (AppSystemProfile.LoginMethod == LoginMethod.VerifyCodeLogin)
                {
                    await this.Dispatcher.DispatchAsync(async () => await SwitchToTelVerifyCodeLoginStyle());
                    Thread.Sleep(500);
                    while (!UserTelEditor.IsFocused)
                    {
                        UserTelEditor.Dispatcher.Dispatch(() => UserTelEditor.Focus());
                        Thread.Sleep(100);
                    }
                }
                else
                {
                    Thread.Sleep(500);
                    while (!UserAccountEditor.IsFocused)
                    {
                        UserAccountBorder.Dispatcher.Dispatch(() => UserAccountEditor.Focus());
                        Thread.Sleep(100);
                    }
                }
            }).StartNewTask();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    public async void ForgetPassword()
    {
        string userTel = await DisplayPromptAsync("忘记密码", "请输入您的手机号", "确定", "取消");
        if (userTel is not null && userTel != string.Empty)
        {
            var xFEExecuter = XCCDataBase.XFEDataBase?.CreateExecuter();
            var result = await xFEExecuter?.ExecuteGet<XFEChatRoom_UserInfoForm>(x => x.Atel == userTel);
            if (result is not null && result.Count == 1)
            {
                //xFEExecuter.ExecuteUpdate<XFEChatRoom_UserInfoForm>(x =>
                //{
                //    return x.Atel == userTel;
                //});
                //ChangeForgetPassword(result);
            }
            else
            {
                await DisplayAlert("绑定异常", "该手机号未注册或绑定账号数异常", "明白了");
            }
        }
    }

    public async void ChangeForgetPassword(List<XFEChatRoom_UserInfoForm> result)
    {
        string userPassword = await DisplayPromptAsync("忘记密码", "请输入您的新密码", "确定");
        if (userPassword is not null && userPassword.VerifyPassword())
        {
            /*result[0].Apassword = userPassword;*/
            UserInfo.CurrentUser = result[0];
            UserInfo.EditUserProperty(UserPropertyToEdit.Password, userPassword);
            if (await UserInfo.UpLoadUserInfo() == 1)
            {
                await DisplayAlert("修改成功", "您的密码已修改", "明白了");
            }
            else { await DisplayAlert("修改失败", "您的密码修改异常", "明白了"); }
        }
        else { await DisplayAlert("输入异常", "您输入的密码不合规", "明白了"); }
    }
    #region 焦点事件
    private void UserAccountEditor_Focused(object sender, FocusEventArgs e)
    {
        if (!isAccountChanged)
        {
            var animation = new Animation(v => UserAccountBorder.WidthRequest = v, 100, 300);
            var animation2 = new Animation(v => UserAccountLabel.WidthRequest = v, 100, 300);
            animation.Commit(this, "UserAccountBorderWidthAnimation", 16, 300, Easing.CubicOut);
            animation2.Commit(this, "UserAccountLabelWidthAnimation", 16, 300, Easing.CubicOut);
            UserAccountLabel.FadeTo(1, 300, Easing.CubicOut);
            UserAccountBorder.FadeTo(1, 300, Easing.CubicOut);
            isAccountChanged = true;
        }
    }

    private void UserAccountEditor_Unfocused(object sender, FocusEventArgs e)
    {
        if (UserAccountEditor.Text is null || UserAccountEditor.Text == string.Empty)
        {
            if (isAccountChanged)
            {
                var animation = new Animation(v => UserAccountBorder.WidthRequest = v, 300, 100);
                var animation2 = new Animation(v => UserAccountLabel.WidthRequest = v, 300, 100);
                animation.Commit(this, "UserAccountBorderWidthAnimation", 16, 200, Easing.CubicOut);
                animation2.Commit(this, "UserAccountLabelWidthAnimation", 16, 200, Easing.CubicOut);
                UserAccountLabel.FadeTo(0.5, 300, Easing.CubicOut);
                UserAccountBorder.FadeTo(0.5, 200, Easing.CubicOut);
                isAccountChanged = false;
            }
        }
    }

    private void UserPasswordEditor_Focused(object sender, FocusEventArgs e)
    {
        if (!isPasswordChanged)
        {
            var animation = new Animation(v => UserPasswordBorder.WidthRequest = v, 100, 300);
            var animation2 = new Animation(v => UserPasswordLabel.WidthRequest = v, 100, 300);
            animation.Commit(this, "UserPasswordBorderWidthAnimation", 16, 300, Easing.CubicOut);
            animation2.Commit(this, "UserPasswordLabelWidthAnimation", 16, 300, Easing.CubicOut);
            UserPasswordLabel.FadeTo(1, 300, Easing.CubicOut);
            UserPasswordBorder.FadeTo(1, 300, Easing.CubicOut);
            isPasswordChanged = true;
        }
    }

    private void UserPasswordEditor_Unfocused(object sender, FocusEventArgs e)
    {
        if (UserPasswordEditor.Text is null || UserPasswordEditor.Text == string.Empty)
        {
            if (isPasswordChanged)
            {
                var animation = new Animation(v => UserPasswordBorder.WidthRequest = v, 300, 100);
                var animation2 = new Animation(v => UserPasswordLabel.WidthRequest = v, 300, 100);
                animation.Commit(this, "UserPasswordBorderWidthAnimation", 16, 200, Easing.CubicOut);
                animation2.Commit(this, "UserPasswordLabelWidthAnimation", 16, 200, Easing.CubicOut);
                UserPasswordLabel.FadeTo(0.5, 300, Easing.CubicOut);
                UserPasswordBorder.FadeTo(0.5, 200, Easing.CubicOut);
                isPasswordChanged = false;
            }
        }
    }

    private void TelVerifyCodeEditor_Focused(object? sender, FocusEventArgs e)
    {
        TelVerifyCodeLabel.FadeTo(1, 300, Easing.CubicOut);
        TelVerifyCodeLabel.ScaleTo(1.2, 300, Easing.CubicOut);
        TelVerifyCodeBorder.FadeTo(1, 300, Easing.CubicOut);
        TelVerifyCodeBorder.ScaleTo(1.2, 300, Easing.CubicOut);
        TelVerifyCodeButton.ScaleTo(0.8, 300, Easing.CubicOut);
    }

    private void TelVerifyCodeEditor_Unfocused(object? sender, FocusEventArgs e)
    {
        TelVerifyCodeLabel.FadeTo(0.5, 300, Easing.CubicOut);
        TelVerifyCodeLabel.ScaleTo(1, 300, Easing.CubicOut);
        TelVerifyCodeBorder.FadeTo(0.5, 300, Easing.CubicOut);
        TelVerifyCodeBorder.ScaleTo(1, 300, Easing.CubicOut);
        TelVerifyCodeButton.ScaleTo(1, 300, Easing.CubicOut);
    }
    private void UserTelEditor_Focused(object? sender, FocusEventArgs e)
    {
        if (!isTelChanged)
        {
            var animation = new Animation(v => UserTelBorder.WidthRequest = v, 100, 300);
            var animation2 = new Animation(v => UserTelLabel.WidthRequest = v, 100, 300);
            animation.Commit(this, "UserAccountBorderWidthAnimation", 16, 300, Easing.CubicOut);
            animation2.Commit(this, "UserAccountLabelWidthAnimation", 16, 300, Easing.CubicOut);
            UserTelLabel.FadeTo(1, 300, Easing.CubicOut);
            UserTelBorder.FadeTo(1, 300, Easing.CubicOut);
            isTelChanged = true;
        }
    }

    private void UserTelEditor_Unfocused(object? sender, FocusEventArgs e)
    {
        if (UserTelEditor.Text is null || UserTelEditor.Text == string.Empty)
        {
            if (isTelChanged)
            {
                var animation = new Animation(v => UserTelBorder.WidthRequest = v, 300, 100);
                var animation2 = new Animation(v => UserTelLabel.WidthRequest = v, 300, 100);
                animation.Commit(this, "UserAccountBorderWidthAnimation", 16, 200, Easing.CubicOut);
                animation2.Commit(this, "UserAccountLabelWidthAnimation", 16, 200, Easing.CubicOut);
                UserTelLabel.FadeTo(0.5, 300, Easing.CubicOut);
                UserTelBorder.FadeTo(0.5, 200, Easing.CubicOut);
                isTelChanged = false;
            }
        }
    }
    #endregion
    #region 编辑框文本改变事件
    private void UserAccountEditor_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(UserAccountEditor.Text))
        {
            UserLoginButton.SetDynamicResource(Button.TextColorProperty, "DisabledMainColor");
            if (!UserLoginButton.IsWaiting)
                UserLoginButton.IsEnabled = false;
            isAccountEditorEmpty = true;
        }
        else
        {
            if (isAccountEditorEmpty && !isPasswordEditorEmpty)
            {
                UserLoginButton.SetDynamicResource(Button.TextColorProperty, "MainColor");
                if (!UserLoginButton.IsWaiting)
                    UserLoginButton.IsEnabled = true;
            }
            isAccountEditorEmpty = false;
        }
    }

    private void UserPasswordEditor_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(UserPasswordEditor.Text))
        {
            UserLoginButton.SetDynamicResource(Button.TextColorProperty, "DisabledMainColor");
            if (!UserLoginButton.IsWaiting)
                UserLoginButton.IsEnabled = false;
            isPasswordEditorEmpty = true;
        }
        else
        {
            if (isPasswordEditorEmpty && !isAccountEditorEmpty)
            {
                UserLoginButton.SetDynamicResource(Button.TextColorProperty, "MainColor");
                if (!UserLoginButton.IsWaiting)
                    UserLoginButton.IsEnabled = true;
            }
            isPasswordEditorEmpty = false;
        }
    }

    private void TelVerifyCodeEditor_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (!isTelEditorEmpty && TelVerifyCodeEditor.Text.Length == 6)
        {
            if (!UserLoginButton.IsWaiting)
            {
                UserLoginButton.SetDynamicResource(Button.TextColorProperty, "MainColor");
                UserLoginButton.IsEnabled = true;
            }
            TelVerifyCodeEditor_Unfocused(null, null!);
        }
        else
        {
            if (!UserLoginButton.IsWaiting)
            {
                UserLoginButton.SetDynamicResource(Button.TextColorProperty, "DisabledMainColor");
                UserLoginButton.IsEnabled = false;
            }
        }
    }
    private void UserTelEditor_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (UserTelEditor.Text.IsMobPhoneNumber())
        {
            isTelEditorEmpty = false;
            UserTelLabel.Text = "手机号";
            UserTelLabel.TextColor = Color.Parse("Black");
            UserTelBorder.SetDynamicResource(Border.StrokeProperty, "DeepDarkGray");
            if (!isCoolDown)
            {
                TelVerifyCodeButton.IsEnabled = true;
                TelVerifyCodeButton.SetDynamicResource(Button.TextColorProperty, "MainColor");
            }
        }
        else
        {
            isTelEditorEmpty = true;
            UserTelLabel.Text = "手机号格式不正确";
            UserTelLabel.TextColor = Color.Parse("Red");
            TelVerifyCodeButton.IsEnabled = false;
            TelVerifyCodeButton.SetDynamicResource(Button.TextColorProperty, "DisabledMainColor");
        }
    }
    #endregion
    #region 按钮点击事件
    private async void UserLoginButton_WaitClick(object? sender, WaitButtonClickedEventArgs e)
    {
        try
        {
            switch (AppSystemProfile.LoginMethod)
            {
                case LoginMethod.PasswordLogin:
                    UserAccountBorder.SetDynamicResource(Border.StrokeProperty, "DeepDarkGray");
                    UserAccountLabel.Text = "手机号/邮箱";
                    UserAccountLabel.TextColor = Color.Parse("Gray");
                    UserPasswordBorder.SetDynamicResource(Border.StrokeProperty, "DeepDarkGray");
                    UserPasswordLabel.Text = "用户密码";
                    UserPasswordLabel.TextColor = Color.Parse("Gray");
                    var mailResult = await XFEExecuter.ExecuteGet<XFEChatRoom_UserInfoForm>(x => x.Amail == UserAccountEditor.Text);
                    var telResult = await XFEExecuter.ExecuteGet<XFEChatRoom_UserInfoForm>(x => x.Atel == UserAccountEditor.Text);
                    if (mailResult is not null && mailResult.Count > 0)
                        await ProcessLoginInfo(mailResult.FirstOrDefault()!, e);
                    else if (telResult is not null && telResult.Count > 0)
                        await ProcessLoginInfo(telResult.FirstOrDefault()!, e);
                    else
                    {
                        ControlExtension.BorderShake(UserAccountBorder);
                        UserAccountLabel.Text = "手机号或邮箱不存在";
                        UserAccountLabel.TextColor = Color.Parse("Red");
                        UserAccountBorder.Stroke = Color.Parse("Red");
                        UserAccountEditor.Focus();
                        e.Continue();
                    }
                    await new Action(() =>
                    {
                        Thread.Sleep(500);
                        e.Continue();
                    }).StartNewTask();
                    break;
                case LoginMethod.VerifyCodeLogin:
                    TelVerifyCodeBorder.SetDynamicResource(Border.StrokeProperty, "DeepDarkGray");
                    TelVerifyCodeLabel.TextColor = Color.Parse("Gray");
                    TelVerifyCodeLabel.Text = "验证码";
                    UserTelBorder.SetDynamicResource(Border.StrokeProperty, "DeepDarkGray");
                    UserTelLabel.TextColor = Color.Parse("Gray");
                    UserTelLabel.Text = "手机号";
                    if (TelVerifyCodeEditor.Text == randomCode)
                    {
                        if (UserTelEditor.Text == currentPhoneNum)
                        {
                            var verifyTelResult = await XFEExecuter.ExecuteGet<XFEChatRoom_UserInfoForm>(x => x.Atel == UserTelEditor.Text);
                            if (verifyTelResult!.Count > 0)
                            {
                                await ProcessLoginInfo(verifyTelResult.FirstOrDefault()!, e);
                                return;
                            }
                            else
                            {
                                UserTelLabel.Text = "手机号不存在，请检查是否填写正确";
                                UserTelLabel.TextColor = Color.Parse("Red");
                                UserTelBorder.Stroke = Color.Parse("Red");
                                ControlExtension.BorderShake(UserTelBorder);
                                UserTelEditor.Focus();
                                e.Continue();
                            }
                        }
                        else
                        {
                            UserTelLabel.TextColor = Color.Parse("Red");
                            UserTelBorder.Stroke = Color.Parse("Red");
                            UserTelLabel.Text = "手机号与验证码发送时的不一致";
                            UserTelEditor.Focus();
                            ControlExtension.BorderShake(UserTelBorder);
                        }
                    }
                    else
                    {
                        TelVerifyCodeLabel.TextColor = Color.Parse("Red");
                        TelVerifyCodeBorder.Stroke = Color.Parse("Red");
                        TelVerifyCodeLabel.Text = "验证码错误";
                        TelVerifyCodeEditor.Focus();
                        ControlExtension.BorderShake(TelVerifyCodeBorder);
                    }
                    break;

                default:
                    await ProcessException.ShowEnumException();
                    break;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("登录时错误", ex.Message, "确认");
        }
    }

    private async Task ProcessLoginInfo(XFEChatRoom_UserInfoForm userInfo, Controls.WaitButtonClickedEventArgs e)
    {
        switch (AppSystemProfile.LoginMethod)
        {
            case LoginMethod.PasswordLogin:
                if (userInfo.Apassword == UserPasswordEditor.Text)
                {
                    await SwitchToLoginSuccessfulStyle(userInfo);
                    e.Continue();
                }
                else
                {
                    ControlExtension.BorderShake(UserPasswordBorder);
                    UserPasswordLabel.Text = "密码输入错误";
                    UserPasswordLabel.TextColor = Color.Parse("Red");
                    UserPasswordBorder.Stroke = Color.Parse("Red");
                    UserPasswordEditor.Text = string.Empty;
                    UserPasswordEditor.Focus();
                    e.Continue();
                }
                break;
            case LoginMethod.VerifyCodeLogin:
                await SwitchToLoginSuccessfulStyle(userInfo);
                e.Continue();
                break;
            default:
                await ProcessException.ShowEnumException();
                break;
        }
    }

    private async Task SwitchToLoginSuccessfulStyle(XFEChatRoom_UserInfoForm userInfo)
    {
        var successfulLabel = new Label
        {
            Text = "登录成功",
            Opacity = 0,
            TextColor = Color.FromArgb("#ffffff"),
            FontAttributes = FontAttributes.Bold,
            FontSize = 40,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center
        };
        this.Content = successfulLabel;
        await successfulLabel.FadeTo(1, 800, Easing.CubicOut);
        UserInfo.IsLoginSuccessful = true;
        UserInfo.StaticUserName = userInfo.Aname;
        UserInfo.StaticUUID = userInfo.ID;
        UserInfo.StaticMail = userInfo.Amail;
        UserInfo.StaticPhoneNum = userInfo.Atel;
        UserInfo.StaticPassword = userInfo.Apassword;
        UserInfo.SaveUserData(this);
        if (UserInfo.CurrentPage is null)
        {
            UserInfo.IsLoginSuccessful = true;
        }
        else
        {
            UserInfo.CurrentPage.charLabel.SetBinding(Label.TextProperty, new Binding("UserName[0]", source: UserInfo.CurrentPage));
            UserInfo.CurrentPage.nameLabel.SetBinding(Label.TextProperty, new Binding("UserName", source: UserInfo.CurrentPage));
            UserInfo.CurrentPage.UserName = userInfo.Aname!;
            UserInfo.CurrentPage.UUID = userInfo.ID!;
            UserInfo.CurrentUser = userInfo;
            UserInfo.CurrentPage.SwitchToLoginStyle();
            CommunityPage.Current?.Refresh();
        }
        await Task.Delay(600);
        await successfulLabel.FadeTo(0, 500, Easing.CubicOut);
        if (UserInfo.CurrentPage is not null)
            UserInfo.CurrentPage.Opacity = 0;
        Shell.Current.SendBackButtonPressed();
        if (UserInfo.CurrentPage is not null)
            await UserInfo.CurrentPage.FadeTo(1, 300, Easing.CubicOut);
    }

    private async void SwitchToRegisterPageButton_WaitClick(object sender, WaitButtonClickedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(UserRegisterPage));
        e.Continue();
    }

    private async void SwitchToTelVerifyCodeLoginButton_WaitClick(object sender, WaitButtonClickedEventArgs e)
    {
        await SwitchToTelVerifyCodeLoginStyle();
        e.Continue();
    }
    private async Task SwitchToTelVerifyCodeLoginStyle()
    {
        AppSystemProfile.LoginMethod = LoginMethod.VerifyCodeLogin;
        AppSystemProfile.SaveSystemProfile();
#pragma warning disable CS4014
        #region FadeAnimation
        UserAccountLabel.TranslateTo(-100, 0, 800, Easing.SpringOut);
        UserAccountBorder.TranslateTo(-100, 0, 800, Easing.SpringOut);
        UserPasswordLabel.TranslateTo(-100, 0, 800, Easing.SpringOut);
        UserPasswordBorder.TranslateTo(-100, 0, 800, Easing.SpringOut);
        UserAccountLabel.FadeTo(0, 800, Easing.SpringOut);
        UserAccountBorder.FadeTo(0, 800, Easing.SpringOut);
        UserPasswordLabel.FadeTo(0, 800, Easing.SpringOut);
        UserPasswordBorder.FadeTo(0, 800, Easing.SpringOut);
        SwitchToTelVerifyCodeLoginButton.FadeTo(0, 800, Easing.SpringOut);
        UserLoginButton.FadeTo(0, 800, Easing.SpringOut);
        ForgotPasswordButton.FadeTo(0, 800, Easing.SpringOut);
        await SwitchToRegisterPageButton.FadeTo(0, 800, Easing.SpringOut);
        #endregion
        #region SetInvisible
        UserAccountLabel.IsVisible = false;
        UserAccountBorder.IsVisible = false;
        UserPasswordLabel.IsVisible = false;
        UserPasswordBorder.IsVisible = false;
        SwitchToTelVerifyCodeLoginButton.IsVisible = false;
        #endregion
        #region SetVisible
        UserTelLabel.TranslationX = 0;
        UserTelLabel.TranslationY = 150;
        UserTelLabel.IsVisible = true;
        UserTelBorder.TranslationX = 0;
        UserTelBorder.TranslationY = 150;
        UserTelBorder.IsVisible = true;
        TelVerifyCodeLabel.TranslationX = 0;
        TelVerifyCodeLabel.TranslationY = 110;
        TelVerifyCodeLabel.IsVisible = true;
        TelVerifyCodeGrid.TranslationX = 0;
        TelVerifyCodeGrid.TranslationY = 110;
        TelVerifyCodeGrid.IsVisible = true;
        ForgotPasswordButton.TranslationX = 0;
        ForgotPasswordButton.TranslationY = 70;
        UserLoginButton.TranslationX = 0;
        UserLoginButton.TranslationY = 50;
        SwitchToPasswordLoginButton.TranslationX = 0;
        SwitchToPasswordLoginButton.TranslationY = 30;
        SwitchToPasswordLoginButton.IsVisible = true;
        SwitchToRegisterPageButton.TranslationX = 0;
        SwitchToRegisterPageButton.TranslationY = 20;
        #endregion
        #region ShowAnimation
        await new Action(() =>
        {
            this.Dispatcher.Dispatch(() =>
            {
                UserTelLabel.FadeTo(0.5, 1000, Easing.CubicOut);
                UserTelBorder.FadeTo(0.5, 1000, Easing.CubicOut);
                UserTelLabel.TranslateTo(0, 0, 1000, Easing.CubicOut);
                UserTelBorder.TranslateTo(0, 0, 1000, Easing.CubicOut);
            });
            Thread.Sleep(200);
            this.Dispatcher.Dispatch(() =>
            {
                TelVerifyCodeGrid.TranslateTo(0, 0, 1000, Easing.CubicOut);
                TelVerifyCodeGrid.FadeTo(1, 1000, Easing.CubicOut);
                TelVerifyCodeLabel.FadeTo(0.5, 1000, Easing.CubicOut);
                TelVerifyCodeLabel.TranslateTo(0, 0, 1000, Easing.CubicOut);
                TelVerifyCodeBorder.FadeTo(0.5, 1000, Easing.CubicOut);
                TelVerifyCodeButton.FadeTo(1, 1000, Easing.CubicOut);
            });
            Thread.Sleep(200);
            this.Dispatcher.Dispatch(() =>
            {
                ForgotPasswordButton.TranslateTo(0, 0, 1000, Easing.CubicOut);
                ForgotPasswordButton.FadeTo(1, 1000, Easing.CubicOut);
            });
            Thread.Sleep(200);
            this.Dispatcher.Dispatch(() =>
            {
                UserLoginButton.TranslateTo(0, 0, 1000, Easing.CubicOut);
                UserLoginButton.FadeTo(1, 1000, Easing.CubicOut);
            });
            Thread.Sleep(200);
            this.Dispatcher.Dispatch(() =>
            {
                SwitchToPasswordLoginButton.TranslateTo(0, 0, 1000, Easing.CubicOut);
                SwitchToPasswordLoginButton.FadeTo(1, 1000, Easing.CubicOut);
            });
            Thread.Sleep(200);
            this.Dispatcher.Dispatch(() =>
            {
                SwitchToRegisterPageButton.TranslateTo(0, 0, 1000, Easing.CubicOut);
                SwitchToRegisterPageButton.FadeTo(1, 1000, Easing.CubicOut);
            });
            Thread.Sleep(200);
        }).StartNewTask();
        UserTelEditor.Focus();
        #endregion
#pragma warning restore CS4014
    }

    private async void SwitchToPasswordLoginButton_WaitClick(object sender, WaitButtonClickedEventArgs e)
    {
        AppSystemProfile.LoginMethod = LoginMethod.PasswordLogin;
        AppSystemProfile.SaveSystemProfile();
#pragma warning disable CS4014
        #region FadeAnimation
        UserTelLabel.TranslateTo(-100, 0, 800, Easing.SpringOut);
        UserTelBorder.TranslateTo(-100, 0, 800, Easing.SpringOut);
        TelVerifyCodeLabel.TranslateTo(-100, 0, 800, Easing.SpringOut);
        TelVerifyCodeGrid.TranslateTo(-100, 0, 800, Easing.SpringOut);
        UserTelLabel.FadeTo(0, 800, Easing.SpringOut);
        UserTelBorder.FadeTo(0, 800, Easing.SpringOut);
        TelVerifyCodeLabel.FadeTo(0, 800, Easing.SpringOut);
        TelVerifyCodeGrid.FadeTo(0, 800, Easing.SpringOut);
        SwitchToPasswordLoginButton.FadeTo(0, 800, Easing.SpringOut);
        UserLoginButton.FadeTo(0, 800, Easing.SpringOut);
        ForgotPasswordButton.FadeTo(0, 800, Easing.SpringOut);
        await SwitchToRegisterPageButton.FadeTo(0, 800, Easing.SpringOut);
        #endregion
        #region SetInvisible
        UserTelLabel.IsVisible = false;
        UserTelBorder.IsVisible = false;
        TelVerifyCodeLabel.IsVisible = false;
        TelVerifyCodeGrid.IsVisible = false;
        SwitchToPasswordLoginButton.IsVisible = false;
        #endregion
        #region SetVisible
        UserAccountLabel.TranslationX = 0;
        UserAccountLabel.TranslationY = 150;
        UserAccountLabel.IsVisible = true;
        UserAccountBorder.TranslationX = 0;
        UserAccountBorder.TranslationY = 150;
        UserAccountBorder.IsVisible = true;
        UserPasswordLabel.TranslationX = 0;
        UserPasswordLabel.TranslationY = 110;
        UserPasswordLabel.IsVisible = true;
        UserPasswordBorder.TranslationX = 0;
        UserPasswordBorder.TranslationY = 110;
        UserPasswordBorder.IsVisible = true;
        ForgotPasswordButton.TranslationX = 0;
        ForgotPasswordButton.TranslationY = 70;
        UserLoginButton.TranslationX = 0;
        UserLoginButton.TranslationY = 50;
        SwitchToTelVerifyCodeLoginButton.TranslationX = 0;
        SwitchToTelVerifyCodeLoginButton.TranslationY = 30;
        SwitchToTelVerifyCodeLoginButton.IsVisible = true;
        SwitchToRegisterPageButton.TranslationX = 0;
        SwitchToRegisterPageButton.TranslationY = 20;
        #endregion
        #region ShowAnimation
        await new Action(() =>
        {
            this.Dispatcher.Dispatch(() =>
            {
                UserAccountLabel.FadeTo(0.5, 1000, Easing.CubicOut);
                UserAccountBorder.FadeTo(0.5, 1000, Easing.CubicOut);
                UserAccountLabel.TranslateTo(0, 0, 1000, Easing.CubicOut);
                UserAccountBorder.TranslateTo(0, 0, 1000, Easing.CubicOut);
            });
            Thread.Sleep(200);
            this.Dispatcher.Dispatch(() =>
            {
                UserPasswordLabel.TranslateTo(0, 0, 1000, Easing.CubicOut);
                UserPasswordLabel.FadeTo(0.5, 1000, Easing.CubicOut);
                UserPasswordBorder.TranslateTo(0, 0, 1000, Easing.CubicOut);
                UserPasswordBorder.FadeTo(0.5, 1000, Easing.CubicOut);
            });
            Thread.Sleep(200);
            this.Dispatcher.Dispatch(() =>
            {
                ForgotPasswordButton.TranslateTo(0, 0, 1000, Easing.CubicOut);
                ForgotPasswordButton.FadeTo(1, 1000, Easing.CubicOut);
            });
            Thread.Sleep(200);
            this.Dispatcher.Dispatch(() =>
            {
                UserLoginButton.TranslateTo(0, 0, 1000, Easing.CubicOut);
                UserLoginButton.FadeTo(1, 1000, Easing.CubicOut);
            });
            Thread.Sleep(200);
            this.Dispatcher.Dispatch(() =>
            {
                SwitchToTelVerifyCodeLoginButton.TranslateTo(0, 0, 1000, Easing.CubicOut);
                SwitchToTelVerifyCodeLoginButton.FadeTo(1, 1000, Easing.CubicOut);
            });
            Thread.Sleep(200);
            this.Dispatcher.Dispatch(() =>
            {
                SwitchToRegisterPageButton.TranslateTo(0, 0, 1000, Easing.CubicOut);
                SwitchToRegisterPageButton.FadeTo(1, 1000, Easing.CubicOut);
            });
            Thread.Sleep(200);
        }).StartNewTask();
        UserAccountEditor.Focus();
        e.Continue();
        #endregion
#pragma warning restore CS4014
    }

    private async void TelVerifyCodeButton_Clicked(object sender, EventArgs e)
    {
        currentPhoneNum = UserTelEditor.Text;
        randomCode = IDGenerator.SummonRandomID(6);
        TelVerifyCodeButton.IsEnabled = false;
        TelVerifyCodeButton.SetDynamicResource(Button.TextColorProperty, "DisabledMainColor");
        TelVerifyCodeButton.Text = "发送中...";
        isCoolDown = true;
        var resp = await TencentSms.SendVerifyCode("1922756", "+86" + UserTelEditor.Text, [randomCode, "2"]);
        if (resp == null || resp.SendStatusSet.First().Code != "Ok")
        {
            await DisplayAlert("出错啦！", $"验证码发送失败：{resp?.SendStatusSet.First().Message}\n手机号：{UserTelEditor.Text}", "啊？");
            TelVerifyCodeButton.IsEnabled = true;
            TelVerifyCodeButton.SetDynamicResource(Button.TextColorProperty, "MainColor");
            TelVerifyCodeButton.Text = "重新发送";
        }
        else
        {
            TelVerifyCodeButton.Text = "重新发送 60";
            await new Action(() =>
            {
                int timer = 60;
                TelVerifyCodeButton.Dispatcher.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    TelVerifyCodeButton.Text = $"重新发送 {--timer}";
                    if (timer == 0)
                    {
                        TelVerifyCodeButton.Text = "重新发送";
                        TelVerifyCodeButton.IsEnabled = true;
                        isCoolDown = false;
                        TelVerifyCodeButton.SetDynamicResource(Button.TextColorProperty, "MainColor");
                        return false;
                    }
                    return true;
                });
            }).StartNewTask();
        }
    }

    private void ForgotPasswordButton_Click(object sender, TappedEventArgs e)
    {
        ForgetPassword();
    }
    #endregion
}
using XFECommunity.AllImpl;
using XFE������չ.NetCore.StringExtension;
using XFE������չ.NetCore.TaskExtension;
using XFE������չ.NetCore.XFEDataBase;

namespace XFECommunity.ViewPage;

public partial class UserRegisterPage : ContentPage
{
    private readonly XFEExecuter XFEExecuter = XCCDataBase.XFEDataBase!.CreateExecuter();
    private bool IsTelEditor = false, IsMailEditor = false, IsNameEditor = false, IsPasswordEditor = false, IsPasswordEnsureEditor = false;
    private string randomCode = string.Empty;
    private string currentPhoneNum = string.Empty;
    private bool IsCoolDown = false;
    public UserRegisterPage()
    {
        InitializeComponent();
        new Action(() =>
        {
            Thread.Sleep(500);
            while (!UserTelEditor.IsFocused)
            {
                UserTelEditor.Dispatcher.Dispatch(() => UserTelEditor.Focus());
                Thread.Sleep(100);
            }
        }).StartNewTask();
    }

    private async void NextStepButton_WaitClick(object sender, Controls.WaitButtonClickedEventArgs e)
    {
        try
        {
            TelVerifyCodeBorder.SetDynamicResource(Border.StrokeProperty, "DeepDarkGray");
            TelVerifyCodeLabel.TextColor = Color.Parse("Gray");
            TelVerifyCodeLabel.Text = "��֤��";
            UserTelBorder.SetDynamicResource(Border.StrokeProperty, "DeepDarkGray");
            UserTelLabel.TextColor = Color.Parse("Gray");
            UserTelLabel.Text = "�ֻ���";
            if (TelVerifyCodeEditor.Text == randomCode)
            {
                if (UserTelEditor.Text == currentPhoneNum)
                {
                    var telResult = await XFEExecuter.ExecuteGet<XFEChatRoom_UserInfoForm>(x => x.Atel == UserTelEditor.Text);
                    if (telResult!.Count > 0)
                    {
                        UserTelLabel.Text = "�ֻ����Ѵ���";
                        UserTelLabel.TextColor = Color.Parse("Red");
                        UserTelBorder.Stroke = Color.Parse("Red");
                        ControlExtension.BorderShake(UserTelBorder);
                        UserTelEditor.Focus();
                        e.Continue();
                        return;
                    }
                    else
                    {
#pragma warning disable CS4014
                        #region FadeAnimation
                        UserTelLabel.TranslateTo(-100, 0, 800, Easing.SpringOut);
                        UserTelBorder.TranslateTo(-100, 0, 800, Easing.SpringOut);
                        NextStepButton.TranslateTo(-100, 0, 800, Easing.SpringOut);
                        TelVerifyCodeButton.TranslateTo(-100, 0, 800, Easing.SpringOut);
                        TelVerifyCodeLabel.TranslateTo(-100, 0, 800, Easing.SpringOut);
                        TelVerifyCodeBorder.TranslateTo(-100, 0, 800, Easing.SpringOut);
                        UserTelLabel.FadeTo(0, 800, Easing.SpringOut);
                        UserTelBorder.FadeTo(0, 800, Easing.SpringOut);
                        TelVerifyCodeLabel.FadeTo(0, 800, Easing.SpringOut);
                        TelVerifyCodeBorder.FadeTo(0, 800, Easing.SpringOut);
                        NextStepButton.FadeTo(0, 800, Easing.SpringOut);
                        SwtichToLoginPageButton.FadeTo(0, 800, Easing.SpringOut);
                        await TelVerifyCodeButton.FadeTo(0, 800, Easing.SpringOut);
                        #endregion
                        #region SetInvisible
                        UserTelLabel.IsVisible = false;
                        UserTelEditor.IsEnabled = false;
                        UserTelBorder.IsVisible = false;
                        TelVerifyCodeLabel.IsVisible = false;
                        TelVerifyCodeEditor.IsEnabled = false;
                        TelVerifyCodeBorder.IsVisible = false;
                        NextStepButton.IsVisible = false;
                        NextStepButton.IsEnabled = false;
                        TelVerifyCodeButton.IsVisible = false;
                        #endregion
                        #region SetVisible
                        UserNameLabel.IsVisible = true;
                        UserNameBorder.IsVisible = true;
                        UserPasswordLabel.IsVisible = true;
                        UserPasswordBorder.IsVisible = true;
                        UserPasswordEnsureLabel.IsVisible = true;
                        UserPasswordEnsureBorder.IsVisible = true;
                        UserMailLabel.IsVisible = true;
                        UserMailBorder.IsVisible = true;
                        UserRegisterButton.IsVisible = true;
                        #endregion
                        #region ShowAnimation
                        await new Action(() =>
                        {
                            this.Dispatcher.Dispatch(() =>
                            {
                                UserNameLabel.FadeTo(0.5, 1000, Easing.CubicOut);
                                UserNameLabel.TranslateTo(0, 0, 1000, Easing.CubicOut);
                                UserNameBorder.FadeTo(0.5, 1000, Easing.CubicOut);
                                UserNameBorder.TranslateTo(0, 0, 1000, Easing.CubicOut);
                            });
                            Thread.Sleep(200);
                            this.Dispatcher.Dispatch(() =>
                            {
                                UserMailLabel.FadeTo(0.5, 1000, Easing.CubicOut);
                                UserMailLabel.TranslateTo(0, 0, 1000, Easing.CubicOut);
                                UserMailBorder.FadeTo(0.5, 1000, Easing.CubicOut);
                                UserMailBorder.TranslateTo(0, 0, 1000, Easing.CubicOut);
                            });
                            Thread.Sleep(200);
                            this.Dispatcher.Dispatch(() =>
                            {
                                UserPasswordLabel.FadeTo(0.5, 1000, Easing.CubicOut);
                                UserPasswordLabel.TranslateTo(0, 0, 1000, Easing.CubicOut);
                                UserPasswordBorder.FadeTo(0.5, 1000, Easing.CubicOut);
                                UserPasswordBorder.TranslateTo(0, 0, 1000, Easing.CubicOut);
                            });
                            Thread.Sleep(200);
                            this.Dispatcher.Dispatch(() =>
                            {
                                UserPasswordEnsureLabel.FadeTo(0.5, 1000, Easing.CubicOut);
                                UserPasswordEnsureLabel.TranslateTo(0, 0, 1000, Easing.CubicOut);
                                UserPasswordEnsureBorder.FadeTo(0.5, 1000, Easing.CubicOut);
                                UserPasswordEnsureBorder.TranslateTo(0, 0, 1000, Easing.CubicOut);
                            });
                            Thread.Sleep(200);
                            this.Dispatcher.Dispatch(() =>
                            {
                                UserRegisterButton.FadeTo(1, 1000, Easing.CubicOut);
                                UserRegisterButton.TranslateTo(0, 0, 1000, Easing.CubicOut);
                            });
                            Thread.Sleep(200);
                            this.Dispatcher.Dispatch(() =>
                            {
                                SwtichToLoginPageButton.FadeTo(1, 1000, Easing.CubicOut);
                            });
                            Thread.Sleep(200);
                        }).StartNewTask();
                        #endregion
#pragma warning restore CS4014
                        UserNameEditor.Focus();
                        e.Continue();
                    }
                }
                else
                {
                    UserTelLabel.TextColor = Color.Parse("Red");
                    UserTelBorder.Stroke = Color.Parse("Red");
                    UserTelLabel.Text = "�ֻ�������֤�뷢��ʱ�Ĳ�һ��";
                    UserTelEditor.Focus();
                    e.Continue();
                    ControlExtension.BorderShake(UserTelBorder);
                }
            }
            else
            {
                TelVerifyCodeLabel.TextColor = Color.Parse("Red");
                TelVerifyCodeBorder.Stroke = Color.Parse("Red");
                TelVerifyCodeLabel.Text = "��֤�����";
                TelVerifyCodeEditor.Focus();
                e.Continue();
                ControlExtension.BorderShake(TelVerifyCodeBorder);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("ע��ʱ���ִ���", ex.Message, "ȷ��");
        }
    }
    private async void TelVerifyCodeButton_Clicked(object sender, EventArgs e)
    {
        randomCode = IDGenerator.SummonRandomID(6);
        currentPhoneNum = UserTelEditor.Text;
        TelVerifyCodeButton.IsEnabled = false;
        TelVerifyCodeButton.SetDynamicResource(Button.TextColorProperty, "DisabledMainColor");
        TelVerifyCodeButton.Text = "������...";
        IsCoolDown = true;
        var resp = await TencentSms.SendVerifyCode("1922756", "+86" + UserTelEditor.Text, [randomCode, "2"]);
        if (resp == null || resp.SendStatusSet.First().Code != "Ok")
        {
            await DisplayAlert("��������", $"��֤�뷢��ʧ�ܣ�{resp?.SendStatusSet.First().Message}\n�ֻ��ţ�{UserTelEditor.Text}", "����");
            TelVerifyCodeButton.IsEnabled = true;
            TelVerifyCodeButton.SetDynamicResource(Button.TextColorProperty, "MainColor");
            TelVerifyCodeButton.Text = "���·���";
        }
        else
        {
            TelVerifyCodeButton.Text = "���·��� 60";
            await new Action(() =>
            {
                int timer = 60;
                TelVerifyCodeButton.Dispatcher.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    TelVerifyCodeButton.Text = $"���·��� {--timer}";
                    if (timer == 0)
                    {
                        TelVerifyCodeButton.Text = "���·���";
                        TelVerifyCodeButton.IsEnabled = true;
                        IsCoolDown = false;
                        TelVerifyCodeButton.SetDynamicResource(Button.TextColorProperty, "MainColor");
                        return false;
                    }
                    return true;
                });
            }).StartNewTask();
        }
    }
    private async void UserRegisterButton_WaitClick(object sender, Controls.WaitButtonClickedEventArgs e)
    {
        string UUID = await IDGenerator.GetCorrectUserUID(XCCDataBase.XFEDataBase!.CreateExecuter());
        try
        {
            var mailResult = await XFEExecuter.ExecuteGet<XFEChatRoom_UserInfoForm>(x => x.Amail == UserMailEditor.Text);
            if (mailResult!.Count > 0)
            {
                UserMailLabel.Text = "�����Ѵ���";
                UserMailLabel.TextColor = Color.Parse("Red");
                UserMailBorder.Stroke = Color.Parse("Red");
                ControlExtension.BorderShake(UserMailBorder);
                UserMailEditor.Focus();
                return;
            }
            var result = await XFEExecuter.ExecuteAdd(new XFEChatRoom_UserInfoForm()
            {
                ID = UUID,
                Aname = UserNameEditor.Text,
                Atel = UserTelEditor.Text,
                Amail = UserMailEditor.Text,
                Apassword = UserPasswordEditor.Text,
            });
            if (result == 0)
            {
                await DisplayAlert("ע��ʧ��", "δ�ܳɹ�ע�ᣬ������", "ȷ��");
                e.Continue();
                return;
            }
            var successfulLabel = new Label
            {
                Text = "ע��ɹ�",
                Opacity = 0,
                FontSize = 40,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            successfulLabel.SetDynamicResource(Label.TextColorProperty, "NormalTextColor");
            this.Content = successfulLabel;
            await successfulLabel.FadeTo(1, 800, Easing.CubicOut);
            await Task.Delay(500);
            await successfulLabel.FadeTo(0, 800, Easing.CubicOut);
            Shell.Current.SendBackButtonPressed();
            e.Continue();
        }
        catch (Exception ex)
        {
            try
            {
                await XFEExecuter.ExecuteDelete<XFEChatRoom_UserInfoForm>(x => x.ID == UUID);
            }
            catch (Exception) { }
            if (await DisplayAlert("ע���������", $"ע��ʧ�ܣ�{ex.Message}", "����", "����"))
            {
                await Console.Out.WriteLineAsync($"�ֻ��ţ�{UserTelEditor.Text}");
                await Console.Out.WriteLineAsync($"���䣺{UserMailEditor.Text}");
                await Console.Out.WriteLineAsync($"���룺{UserPasswordEditor.Text}");
                await Console.Out.WriteLineAsync($"ȷ�����룺{UserPasswordEnsureEditor.Text}");
                await Console.Out.WriteLineAsync($"��֤�룺{TelVerifyCodeEditor.Text}");
                await Console.Out.WriteLineAsync($"����룺{randomCode}");
                await Console.Out.WriteLineAsync($"�û�����{UserNameEditor.Text}");
                await Console.Out.WriteLineAsync(ex.ToString());
                e.Continue();
            }
            else
            {
                await Shell.Current.GoToAsync("..");
                e.Continue();
            }
        }
    }
    private async void SwitchToLoginPageButton_WaitClick(object sender, Controls.WaitButtonClickedEventArgs e)
    {
        await Shell.Current.GoToAsync("..");
        e.Continue();
    }
    #region �༭�����ݼ��
    private void UserTelEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (UserTelEditor.Text.IsMobPhoneNumber())
        {
            IsTelEditor = true;
            UserTelLabel.Text = "�ֻ���";
            UserTelLabel.TextColor = Color.Parse("Black");
            UserTelBorder.SetDynamicResource(Border.StrokeProperty, "DeepDarkGray");
            if (!IsCoolDown)
            {
                TelVerifyCodeButton.IsEnabled = true;
                TelVerifyCodeButton.SetDynamicResource(Button.TextColorProperty, "MainColor");
            }
        }
        else
        {
            IsTelEditor = false;
            UserTelLabel.Text = "�ֻ��Ÿ�ʽ����ȷ";
            UserTelLabel.TextColor = Color.Parse("Red");
            TelVerifyCodeButton.IsEnabled = false;
            TelVerifyCodeButton.SetDynamicResource(Button.TextColorProperty, "DisabledMainColor");
        }
    }

    private void UserMailEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (UserMailEditor.Text.IsValidEmail())
        {
            IsMailEditor = true;
            UserMailLabel.Text = "����";
            UserMailLabel.TextColor = Color.Parse("Black");
            UserMailBorder.SetDynamicResource(Border.StrokeProperty, "DeepDarkGray");
        }
        else
        {
            IsMailEditor = false;
            UserMailLabel.Text = "�����ʽ����ȷ";
            UserMailLabel.TextColor = Color.Parse("Red");
        }
        CheckCorrect();
    }
    private void UserNameEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (!string.IsNullOrEmpty(UserNameEditor.Text))
        {
            if (!UserNameEditor.Text.Contains(' '))
            {
                IsNameEditor = true;
                UserNameLabel.Text = "�ǳ�";
                UserNameLabel.TextColor = Color.Parse("Black");
            }
            else
            {
                UserNameLabel.Text = "�ǳƲ��ɰ����ո�";
                UserNameLabel.TextColor = Color.Parse("Red");
                IsNameEditor = false;
            }
        }
        else
        {
            UserNameLabel.Text = "�ǳƲ���Ϊ��";
            UserNameLabel.TextColor = Color.Parse("Red");
            IsNameEditor = false;
        }
        CheckCorrect();
    }

    private void UserPasswordEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (!string.IsNullOrEmpty(UserPasswordEditor.Text))
        {
            if (!UserPasswordEditor.Text.Contains(' '))
            {
                IsPasswordEditor = true;
                UserPasswordLabel.Text = "����";
                UserPasswordLabel.TextColor = Color.Parse("Black");
            }
            else
            {
                UserPasswordLabel.Text = "���벻�ɰ����ո�";
                UserPasswordLabel.TextColor = Color.Parse("Red");
                IsPasswordEditor = false;
            }
        }
        else
        {
            UserPasswordLabel.Text = "���벻��Ϊ��";
            UserPasswordLabel.TextColor = Color.Parse("Red");
            IsPasswordEditor = false;
        }
        CheckCorrect();
    }

    private void UserPasswordEnsureEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (UserPasswordEnsureEditor.Text == UserPasswordEditor.Text)
        {
            IsPasswordEnsureEditor = true;
            UserPasswordEnsureLabel.Text = "ȷ������";
            UserPasswordEnsureLabel.TextColor = Color.Parse("Black");
        }
        else
        {
            IsPasswordEnsureEditor = false;
            UserPasswordEnsureLabel.Text = "�������벻һ��";
            UserPasswordEnsureLabel.TextColor = Color.Parse("Red");
        }
        CheckCorrect();
    }

    private void TelVerifyCodeEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (IsTelEditor && TelVerifyCodeEditor.Text.Length == 6)
        {
            if (!NextStepButton.IsWaiting)
            {
                NextStepButton.SetDynamicResource(Button.TextColorProperty, "MainColor");
                NextStepButton.IsEnabled = true;
            }
            TelVerifyCodeEditor_Unfocused(null, null!);
        }
        else
        {
            if (!NextStepButton.IsWaiting)
            {
                NextStepButton.SetDynamicResource(Button.TextColorProperty, "DisabledMainColor");
                NextStepButton.IsEnabled = false;
            }
        }
    }

    private void CheckCorrect()
    {
        if (IsTelEditor && IsMailEditor && IsNameEditor && IsPasswordEditor && IsPasswordEnsureEditor)
        {
            UserRegisterButton.SetDynamicResource(Button.TextColorProperty, "MainColor");
            if (!UserRegisterButton.IsWaiting)
                UserRegisterButton.IsEnabled = true;
        }
        else
        {
            UserRegisterButton.SetDynamicResource(Button.TextColorProperty, "DisabledMainColor");
            if (!UserRegisterButton.IsWaiting)
                UserRegisterButton.IsEnabled = false;
        }
    }
    #endregion
    #region �༭�򽹵��¼�
    private void UserTelEditor_Focused(object? sender, FocusEventArgs e)
    {
        UserTelLabel.FadeTo(1, 300, Easing.CubicOut);
        UserTelLabel.ScaleTo(1.2, 300, Easing.CubicOut);
        UserTelBorder.FadeTo(1, 300, Easing.CubicOut);
        UserTelBorder.ScaleTo(1.2, 300, Easing.CubicOut);
    }

    private void UserTelEditor_Unfocused(object? sender, FocusEventArgs e)
    {
        UserTelLabel.FadeTo(0.5, 300, Easing.CubicOut);
        UserTelLabel.ScaleTo(1, 300, Easing.CubicOut);
        UserTelBorder.FadeTo(0.5, 300, Easing.CubicOut);
        UserTelBorder.ScaleTo(1, 300, Easing.CubicOut);
    }

    private void UserMailEditor_Focused(object? sender, FocusEventArgs e)
    {
        UserMailLabel.FadeTo(1, 300, Easing.CubicOut);
        UserMailLabel.ScaleTo(1.2, 300, Easing.CubicOut);
        UserMailBorder.FadeTo(1, 300, Easing.CubicOut);
        UserMailBorder.ScaleTo(1.2, 300, Easing.CubicOut);
    }

    private void UserMailEditor_Unfocused(object? sender, FocusEventArgs e)
    {
        UserMailLabel.FadeTo(0.5, 300, Easing.CubicOut);
        UserMailLabel.ScaleTo(1, 300, Easing.CubicOut);
        UserMailBorder.FadeTo(0.5, 300, Easing.CubicOut);
        UserMailBorder.ScaleTo(1, 300, Easing.CubicOut);
    }

    private void UserNameEditor_Focused(object? sender, FocusEventArgs e)
    {
        UserNameLabel.FadeTo(1, 300, Easing.CubicOut);
        UserNameLabel.ScaleTo(1.2, 300, Easing.CubicOut);
        UserNameBorder.FadeTo(1, 300, Easing.CubicOut);
        UserNameBorder.ScaleTo(1.2, 300, Easing.CubicOut);
    }

    private void UserNameEditor_Unfocused(object? sender, FocusEventArgs e)
    {
        UserNameLabel.FadeTo(0.5, 300, Easing.CubicOut);
        UserNameLabel.ScaleTo(1, 300, Easing.CubicOut);
        UserNameBorder.FadeTo(0.5, 300, Easing.CubicOut);
        UserNameBorder.ScaleTo(1, 300, Easing.CubicOut);
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

    private void UserPasswordEditor_Focused(object? sender, FocusEventArgs e)
    {
        UserPasswordLabel.FadeTo(1, 300, Easing.CubicOut);
        UserPasswordLabel.ScaleTo(1.2, 300, Easing.CubicOut);
        UserPasswordBorder.FadeTo(1, 300, Easing.CubicOut);
        UserPasswordBorder.ScaleTo(1.2, 300, Easing.CubicOut);
    }

    private void UserPasswordEditor_Unfocused(object? sender, FocusEventArgs e)
    {
        UserPasswordLabel.FadeTo(0.5, 300, Easing.CubicOut);
        UserPasswordLabel.ScaleTo(1, 300, Easing.CubicOut);
        UserPasswordBorder.FadeTo(0.5, 300, Easing.CubicOut);
        UserPasswordBorder.ScaleTo(1, 300, Easing.CubicOut);
    }

    private void UserPasswordEnsureEditor_Focused(object? sender, FocusEventArgs e)
    {
        UserPasswordEnsureLabel.FadeTo(1, 300, Easing.CubicOut);
        UserPasswordEnsureLabel.ScaleTo(1.2, 300, Easing.CubicOut);
        UserPasswordEnsureBorder.FadeTo(1, 300, Easing.CubicOut);
        UserPasswordEnsureBorder.ScaleTo(1.2, 300, Easing.CubicOut);
    }

    private void UserPasswordEnsureEditor_Unfocused(object? sender, FocusEventArgs e)
    {
        UserPasswordEnsureLabel.FadeTo(0.5, 300, Easing.CubicOut);
        UserPasswordEnsureLabel.ScaleTo(1, 300, Easing.CubicOut);
        UserPasswordEnsureBorder.FadeTo(0.5, 300, Easing.CubicOut);
        UserPasswordEnsureBorder.ScaleTo(1, 300, Easing.CubicOut);
    }
    #endregion
}
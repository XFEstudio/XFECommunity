using MauiPopup;
using XFECommunity.AllImpl;
using XFECommunity.Controls;
using XFE������չ.ArrayExtension;
using XFE������չ.ListExtension;
using XFE������չ.NetCore.XFEDataBase;
using XFE������չ.TaskExtension;

namespace XFECommunity.ViewPage;

public partial class PostEditPage : ContentPage
{
    public XFEChatRoom_CommunityPost CurrentPostData { get; set; }
    private XFEExecuter XFEExecuter = XCCDataBase.XFEDataBase.CreateExecuter();
    private List<string> tags = new List<string>();
    private bool BackTrigger = false;
    private bool SecTrigger = false;
    private bool Posting = false;
    private bool Deleting = false;
    public PostEditPage()
    {
        InitializeComponent();
        CurrentPostData = PostViewPage.Current?.CurrentPostData;
        if (CurrentPostData is not null)
        {
            TitleEditor.Text = CurrentPostData.PostTitle;
            ContentEditor.Text = CurrentPostData.PostContent;
            foreach (var tag in CurrentPostData.PostTag.ToXFEArray<string>())
            {
                if (tags.Contains(tag))
                    continue;
                tags.Add(tag);
                var button = new Button
                {
                    Text = $"#{tag}",
                    BackgroundColor = Color.FromArgb("#F0ECFE"),
                    TextColor = Color.FromArgb("#512BD4"),
                    CornerRadius = 30,
                    Margin = new Thickness(10, 0, 0, 0)
                };
                button.Clicked += TagButton_Clicked;
                TagStackLayout.Children.Add(button);
            }
            ToolbarItems.Add(new ToolbarItem
            {
                Text = "ɾ��",
                IconImageSource = "trash",
                Command = new Command(async () =>
                {
                    if (Deleting)
                    {
                        return;
                    }
                    Deleting = true;
                    BackTrigger = true;
                    if (await DisplayAlert("ɾ������", "ȷ��ɾ����ɾ��������Ӳ��ɻָ�", "ȷ��", "ȡ��"))
                        try
                        {
                            var task = PopupAction.DisplayPopup(new TipPopup("ɾ����...", 300));
                            var tarPost = await XFEExecuter.ExecuteGetFirst<XFEChatRoom_CommunityPost>(x => x.PostID == CurrentPostData.PostID);
                            var result = await XFEExecuter.ExecuteDelete(tarPost);
                            if (result == 0)
                            {
                                await PopupAction.ClosePopup();
                                BackTrigger = false;
                                Deleting = false;
                                await PopupAction.DisplayPopup(new ErrorPopup("ɾ��ʧ��", "�����������ò��������·���"));
                                return;
                            }
                            await Task.Delay(800);
                            await PopupAction.ClosePopup();
                            await this.Content.FadeTo(0, 300, Easing.CubicInOut);
                            var successfulLabel = new Label
                            {
                                Text = "ɾ���ɹ�",
                                Opacity = 0,
                                TextColor = Color.FromArgb("#512BD4"),
                                FontAttributes = FontAttributes.Bold,
                                FontSize = 40,
                                VerticalOptions = LayoutOptions.Center,
                                HorizontalOptions = LayoutOptions.Center
                            };
                            this.Content = successfulLabel;
                            await successfulLabel.FadeTo(1, 300, Easing.CubicInOut);
                            await Task.Delay(1000);
                            await successfulLabel.FadeTo(0, 300, Easing.CubicInOut);
                            PostViewPage.Current.CurrentPostData = null;
                            await Shell.Current.GoToAsync("../..");
                            CommunityPage.Current.RemovePostByID(CurrentPostData?.PostID);
                            CommunityPage.Current.postRefreshView_Refreshing(null, null);
                            BackTrigger = false;
                            Deleting = false;
                        }
                        catch (Exception ex)
                        {
                            await PopupAction.ClosePopup();
                            BackTrigger = false;
                            Deleting = false;
                            await PopupAction.DisplayPopup(new ErrorPopup("ɾ��ʧ��", $"�����������ò��������·���{ex.Message}"));
                            return;
                        }
                    else
                        Deleting = false;
                })
            });
        }
        ToolbarItems.Add(new ToolbarItem
        {
            Text = "����",
            Command = new Command(async () =>
            {
                if (Posting)
                {
                    return;
                }
                if (string.IsNullOrWhiteSpace(TitleEditor.Text))
                {
                    await DisplayAlert("��Ŷ��QAQ", "���ⲻ��Ϊ��", "ȷ��");
                    return;
                }
                if (string.IsNullOrWhiteSpace(ContentEditor.Text))
                {
                    await DisplayAlert("��Ŷ��QAQ", "���ݲ���Ϊ��", "ȷ��");
                    return;
                }
                if (ContentEditor.Text.Length > 3000)
                {
                    await DisplayAlert("��Ŷ��QAQ", "������������\n��������������δ��ɵĴ���Ŷ~", "ȷ��");
                    return;
                }
                BackTrigger = true;
                Posting = true;
                var task = PopupAction.DisplayPopup(new TipPopup("������...", 300));
                var timeSpend = new Action(async () =>
                {
                    if (CurrentPostData is null)
                    {
                        try
                        {
                            var result = await XFEExecuter.ExecuteAdd(new XFEChatRoom_CommunityPost
                            {
                                PostTitle = TitleEditor.Text,
                                PostContent = ContentEditor.Text,
                                UName = UserInfo.StaticUserName,
                                UID = UserInfo.StaticUUID,
                                PostID = await IDGenerator.GetCorrectPostID(XFEExecuter),
                                PostTag = tags.ToXFEString()
                            });
                            if (result == 0)
                            {
                                await PopupAction.ClosePopup();
                                BackTrigger = false;
                                Posting = false;
                                await PopupAction.DisplayPopup(new ErrorPopup("����ʧ��", "�����������ò��������·���"));
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            await PopupAction.ClosePopup();
                            BackTrigger = false;
                            Posting = false;
                            await PopupAction.DisplayPopup(new ErrorPopup("����ʧ��", $"�����������ò��������·���{ex.Message}"));
                            return;
                        }
                    }
                    else
                    {
                        CurrentPostData = await XFEExecuter.ExecuteGetFirst<XFEChatRoom_CommunityPost>(x => x.PostID == CurrentPostData.PostID);
                        CurrentPostData.PostTitle = TitleEditor.Text;
                        CurrentPostData.PostContent = ContentEditor.Text;
                        CurrentPostData.PostTag = tags.ToXFEString();
                        try
                        {
                            var result = await XFEExecuter.ExecuteUpdate(CurrentPostData);
                            if (result == 0)
                            {
                                await PopupAction.ClosePopup();
                                BackTrigger = false;
                                Posting = false;
                                await PopupAction.DisplayPopup(new ErrorPopup("�����༭ʧ��", "�����������ò��������·���"));
                                return;
                            }
                            await PostViewPage.Current.Refresh();
                        }
                        catch (Exception ex)
                        {
                            await PopupAction.ClosePopup();
                            BackTrigger = false;
                            Posting = false;
                            await PopupAction.DisplayPopup(new ErrorPopup("����ʧ��", $"�����������ò��������·���{ex.Message}"));
                            return;
                        }
                    }
                }).CTime(true, "��������").TotalSeconds;
                if (timeSpend < 1)
                    await Task.Delay(800);
                await PopupAction.ClosePopup();
                await this.Content.FadeTo(0, 300, Easing.CubicInOut);
                var successfulLabel = new Label
                {
                    Text = "�����ɹ�",
                    Opacity = 0,
                    TextColor = Color.FromArgb("#512BD4"),
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 40,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                };
                this.Content = successfulLabel;
                await successfulLabel.FadeTo(1, 300, Easing.CubicInOut);
                await Task.Delay(1000);
                await successfulLabel.FadeTo(0, 300, Easing.CubicInOut);
                CommunityPage.Current.postRefreshView_Refreshing(null, null);
                if (CurrentPostData is not null)
                {
                    PostViewPage.Current?.Refresh();
                }
                Shell.Current.SendBackButtonPressed();
                Posting = false;
            })
        });
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
#if ANDROID
        (TitleEditor.Handler.PlatformView as Android.Widget.EditText).Background = null;
        (ContentEditor.Handler.PlatformView as Android.Widget.EditText).Background = null;
#endif
    }
    protected override bool OnBackButtonPressed()
    {
        if (!BackTrigger && !SecTrigger)
        {
            this.Dispatcher.Dispatch(async () =>
            {
                if (await DisplayAlert("ȷ���˳���", "δ��������ݽ��ᶪʧ�������ѡ��", "�˳�", "ȡ��"))
                {
                    SecTrigger = true;
                    Shell.Current.SendBackButtonPressed();
                }
                else
                {
                    SecTrigger = false;
                }
            });
            return true;
        }
        else
        {
            return base.OnBackButtonPressed();
        }
    }

    private void ContentEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (ContentEditor.Text.Length > 3000)
        {
            WordCountLabel.TextColor = Color.Parse("Red");
            WordCountLabel.Opacity = 1;
        }
        else
        {
            WordCountLabel.TextColor = Color.Parse("Gray");
            WordCountLabel.Opacity = 0.5;
        }
        WordCountLabel.Text = $"������{ContentEditor.Text.Length}/3000";
    }

    private async void AddTagButton_Clicked(object sender, EventArgs e)
    {
        var str = await DisplayPromptAsync("��ӱ�ǩ", "�������ǩ����", "ȷ��", "ȡ��", "����Ϊ�ջ��пո�", 10, null);
        if (str is null)
            return;
        if (string.IsNullOrWhiteSpace(str) || str.Contains(' '))
        {
            await DisplayAlert("Ŷ��QAQ", "��ǩ���Ʋ���Ϊ�ջ��пո�", "ȷ��");
            return;
        }
        if (tags.Contains(str))
        {
            await DisplayAlert("Ŷ��QAQ", "��ǩ�����ظ�", "�ð�");
            return;
        }
        tags.Add(str);
        var button = new Button
        {
            Text = $"#{str}",
            BackgroundColor = Color.FromArgb("#F0ECFE"),
            TextColor = Color.FromArgb("#512BD4"),
            CornerRadius = 30,
            Margin = new Thickness(10, 0, 0, 0)
        };
        button.Clicked += TagButton_Clicked;
        TagStackLayout.Children.Add(button);
    }
    private void TagButton_Clicked(object sender, EventArgs e)
    {
        TagStackLayout.Children.Remove(sender as Button);
    }
}
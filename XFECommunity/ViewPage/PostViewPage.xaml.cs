using XFECommunity.AllImpl;
using XFECommunity.Controls;
using XFE������չ.NetCore.ArrayExtension;
using XFE������չ.NetCore.TaskExtension;
using XFE������չ.NetCore.XFEDataBase;

namespace XFECommunity.ViewPage;

[QueryProperty(nameof(PostID), nameof(PostID))]
public partial class PostViewPage : ContentPage
{
    public readonly BindableProperty LikeCountProperty = BindableProperty.Create(nameof(LikeCount), typeof(int), typeof(PostViewPage), 0);
    public readonly BindableProperty StarCountProperty = BindableProperty.Create(nameof(StarCount), typeof(int), typeof(PostViewPage), 0);
    public int LikeCount
    {
        get => (int)GetValue(LikeCountProperty);
        set => SetValue(LikeCountProperty, value);
    }
    public int StarCount
    {
        get => (int)GetValue(StarCountProperty);
        set => SetValue(StarCountProperty, value);
    }
    private string postID;

    public string PostID
    {
        get { return postID; }
        set
        {
            if (string.IsNullOrEmpty(value))
                return;
            postID = value;
            if (!Initialized)
                InitializePageData();
        }
    }
    public XFEChatRoom_CommunityPost CurrentPostData { get; set; }
    public static PostViewPage Current { get; private set; }
    private readonly List<string> commentIDList = [];
    private readonly List<CommentCardView> commentCardList = [];
    private bool Editing = false;
    private bool Initialized = false;
    private bool RefreshingIsBusy = false;
    private long totalHeight = 0;
    private string QuoteID = string.Empty;
    private readonly XFEExecuter XFEExecuter = XCCDataBase.XFEDataBase.CreateExecuter();
    public PostViewPage()
    {
        InitializeComponent();
        this.BindingContext = this;
        Current = this;
        backButton.Command = new Command(() =>
        {
            Shell.Current.SendBackButtonPressed();
        });
    }

    protected override bool OnBackButtonPressed()
    {
        try { XFEExecuter.Dispose(); } catch (Exception) { }
        CurrentPostData = null;
        return base.OnBackButtonPressed();
    }

    private async void InitializePageData()
    {
        Initialized = true;
        await Refresh();
        if (CurrentPostData.UID == UserInfo.CurrentUser.ID)
        {
            ToolbarItems.Add(new ToolbarItem
            {
                Text = "�༭",
                Command = new Command(async () =>
                {
                    if (Editing)
                    {
                        return;
                    }
                    Editing = true;
                    await Shell.Current.GoToAsync(nameof(PostEditPage));
                    Editing = false;
                })
            });
        }
    }

    public async Task Refresh()
    {
        try
        {
            XFEExecuter.RefreshExecuter();
            LoadComment();
            CurrentPostData = await XFEExecuter.ExecuteGetFirst<XFEChatRoom_CommunityPost>(x => x.PostID == PostID);
            if (CurrentPostData is null)
            {
                await DisplayAlert("Ŷ��", "�޷���ȡ������Ϣ\n����ID��" + PostID, "����");
                return;
            }
            this.Title = "С�ѣ�" + (CurrentPostData.PostTitle.Length > 10 ? CurrentPostData.PostTitle[..10] + "..." : CurrentPostData.PostTitle);
            TitleLabel.Text = CurrentPostData.PostTitle;
            charLabel.Text = CurrentPostData.UName[0].ToString();
            ContentLabel.Text = CurrentPostData.PostContent;
            AuthorLabel.Text = CurrentPostData.UName;
            TimeLabel.Text = CurrentPostData.PostTime.ToString();
            LikeCount = CurrentPostData.PostLike;
            StarCount = CurrentPostData.PostStar;
            if (UserInfo.IsLoginSuccessful)
            {
                LikeButton.IsLike = UserInfo.CurrentUser.LikedPostID.Contains(CurrentPostData.PostID);
                StarButton.IsStar = UserInfo.CurrentUser.StarredPostID.Contains(CurrentPostData.PostID);
            }
            tagStackLayout.Clear();
            foreach (var tag in CurrentPostData.PostTag.ToXFEArray<string>())
            {
                var button = new Button
                {
                    Text = $"#{tag}",
                    BackgroundColor = Color.FromArgb("#F0ECFE"),
                    TextColor = Color.FromArgb("#512BD4"),
                    CornerRadius = 30,
                    Margin = new Thickness(5, 3, 0, 3)
                };
                button.Clicked += TagButton_Clicked;
                tagStackLayout.Children.Add(button);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ŷ��", "�޷���ȡ������Ϣ\n����ID��" + PostID + "\n" + ex.Message, "����");
        }
    }

    private async void LoadComment()
    {
        if (RefreshingIsBusy)
            return;
        RefreshingIsBusy = true;
        int getCommentRetryTime = 0;
        int getQuoteRetryTime = 0;
        await new Action(async () =>
        {
            using (var Executer = XCCDataBase.XFEDataBase.CreateExecuter())
            {
                List<XFEChatRoom_CommunityComment> commentDataList = new List<XFEChatRoom_CommunityComment>();
                while (getCommentRetryTime < 30)
                {
                    try
                    {
                        commentDataList = await AppAlgorithm.GetPostComment(PostID, 10, commentIDList);
                        getCommentRetryTime = 0;
                        break;
                    }
                    catch (Exception)
                    {
                        getCommentRetryTime++;
                        continue;
                    }
                }
                if (commentDataList is not null)
                {
                    if (commentDataList.Count > 0)
                    {
                        this.Dispatcher.Dispatch(() =>
                        {
                            NoneCommentLabel.IsVisible = false;
                        });
                        foreach (var commentData in commentDataList)
                        {
                            var tarCommentCard = commentCardList.Find(x => x.CommentID == commentData.CommentID);
                            XFEChatRoom_CommunityComment quoteCommentData = null;
                            while (getQuoteRetryTime < 30)
                            {
                                try
                                {
                                    quoteCommentData = await Executer.ExecuteGetFirst<XFEChatRoom_CommunityComment>(x => x.CommentID == commentData.QuoteID);
                                    break;
                                }
                                catch (Exception)
                                {
                                    if (getQuoteRetryTime == 29)
                                    {
                                        quoteCommentData = null;
                                        await DisplayAlert("����ʧ��", $"�޷���ȡ���۵������б�", "ȷ��");
                                        break;
                                    }
                                    getQuoteRetryTime++;
                                    continue;
                                }
                            }
                            if (tarCommentCard is not null)
                            {
                                tarCommentCard.ReloadData(commentData, quoteCommentData);
                                tarCommentCard.Dispatcher.Dispatch(() =>
                                {
                                    if (UserInfo.IsLoginSuccessful && UserInfo.CurrentUser.LikedCommentID.Contains(tarCommentCard.CommentID))
                                    {
                                        tarCommentCard.IsLike = true;
                                    }
                                });
                            }
                            else
                            {
                                var commentCard = new CommentCardView(commentData, quoteCommentData);
                                commentCard.Margin = new Thickness(0, 5);
                                commentCard.LikeClick += CommentCard_LikeClick;
                                commentCard.QuoteClick += CommentCard_QuoteClick;
                                commentCard.CommentCardTapped += CommentCard_CommentCardTapped;
                                if (UserInfo.IsLoginSuccessful && UserInfo.CurrentUser.LikedCommentID.Contains(commentData.CommentID))
                                {
                                    commentCard.IsLike = true;
                                }
                                commentCardList.Add(commentCard);
                                commentIDList.Add(commentCard.CommentID);
                                CommentStack.Dispatcher.Dispatch(() =>
                                {
                                    CommentStack.Add(commentCard);
                                });
                                totalHeight = GetTotalHeight();
                            }
                        }
                    }
                    else
                    {
                        this.Dispatcher.Dispatch(() =>
                        {
                            NoneCommentLabel.IsVisible = true;
                        });
                    }
                }
                else
                {
                    await DisplayAlert("����ʧ��", $"�޷���ȡ�����б�", "ȷ��");
                }
            }
            RefreshingIsBusy = false;
        }).StartNewTask();
    }

    private void CommentCard_CommentCardTapped(object sender, EventArgs e)
    {
        if (sender is CommentCardView commentCard)
        {
            SwitchToQuoteStyle(commentCard.CommentID);
            InputEditor.Focus();
        }
    }

    private void CommentCard_QuoteClick(object sender, CommentCardQuoteClickEventArgs e)
    {
        if (sender is CommentCardView commentCard)
        {
            SwitchToQuoteStyle(commentCard.CommentID);
            InputEditor.Focus();
        }
    }

    private async void CommentCard_LikeClick(object sender, CommentCardLikeClickEventArgs e)
    {
        if (!UserInfo.IsLoginSuccessful)
        {
            if (await DisplayAlert("����", "���ȵ�¼Ŷ", "ǰ����¼", "ȡ��"))
            {
                await Shell.Current.GoToAsync(nameof(UserLoginPage));
            }
            return;
        }
        if (sender is CommentCardView commentCard)
            if (commentCard.IsLike)
            {
                try
                {
                    UserInfo.CurrentUser.LikedCommentID += new string[] { commentCard.CommentID }.ToXFEString();
                    await UserInfo.UpLoadUserInfo();
                    if (await commentCard.CurrentCommentData.ExecuteUpdate(XFEExecuter) == 0)
                    {
                        commentCard.IsLike = false;
                        await DisplayAlert("����ʧ��", "������������", "ȷ��");
                    }
                }
                catch (Exception ex)
                {
                    commentCard.IsLike = false;
                    await DisplayAlert("����ʧ��", "������������" + ex.Message, "ȷ��");
                }
            }
            else
            {
                try
                {
                    UserInfo.CurrentUser.LikedCommentID = UserInfo.CurrentUser.LikedCommentID.Replace($"[+-{commentCard.CommentID}-+]", string.Empty);
                    await UserInfo.UpLoadUserInfo();
                    if (await commentCard.CurrentCommentData.ExecuteUpdate(XFEExecuter) == 0)
                    {
                        commentCard.IsLike = true;
                        await DisplayAlert("ȡ������ʧ��", "������������", "ȷ��");
                    }
                }
                catch (Exception ex)
                {
                    commentCard.IsLike = true;
                    await DisplayAlert("ȡ������ʧ��", "������������" + ex.Message, "ȷ��");
                }
            }
    }

    private long GetTotalHeight()
    {
        long totalHeight = 0;
        foreach (var commentCard in commentCardList)
        {
            totalHeight += (long)commentCard.DesiredSize.Height + 12;
        }
        return totalHeight;
    }

    private async void TagButton_Clicked(object sender, EventArgs e)
    {
        await DisplayAlert("��ǩ", $"{(sender as Button).Text[1..]}", "ȷ��");
    }

    private void InputEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(InputEditor.Text))
        {
            SendButton.BackgroundColor = Color.FromArgb("#A491E8");
            SendButton.IsEnabled = false;
        }
        else
        {
            SendButton.BackgroundColor = Color.FromArgb("#512BD4");
            SendButton.IsEnabled = true;
        }
    }

    private void SwitchToQuoteStyle(string quoteID)
    {
        QuoteID = quoteID;
        var tarComment = commentCardList.Find(x => x.CommentID == quoteID);
        QuoteLabel.Text = $"{tarComment.UserName}��{tarComment.CommentContent}";
        QuoteBorder.IsVisible = true;
    }

    private async void SendButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (!UserInfo.IsLoginSuccessful)
            {
                if (await DisplayAlert("����", "���ȵ�¼Ŷ", "ǰ����¼", "ȡ��"))
                {
                    await Shell.Current.GoToAsync(nameof(UserLoginPage));
                }
                return;
            }
            SendButton.IsEnabled = false;
            SendButton.BackgroundColor = Color.FromArgb("#A491E8");
            InputEditor.IsEnabled = false;
            var tarCommentId = await IDGenerator.GetCorrectCommentId(XFEExecuter);
            try
            {
                var result = await XFEExecuter.ExecuteAdd(new XFEChatRoom_CommunityComment
                {
                    CommentID = tarCommentId,
                    PostID = PostID,
                    CommentContent = InputEditor.Text,
                    UID = UserInfo.CurrentUser.ID,
                    UName = UserInfo.CurrentUser.Aname,
                    FloorCount = await AppAlgorithm.GetCommentFloor(PostID, XFEExecuter),
                    QuoteID = QuoteID
                });
                if (result == 0)
                {
                    SendButton.BackgroundColor = Color.FromArgb("#512BD4");
                    InputEditor.IsEnabled = true;
                    SendButton.IsEnabled = true;
                    await PopupAction.DisplayPopup(new ErrorPopup("����ʧ��", "������������"));
                    return;
                }
            }
            catch (Exception) { }
            XFEExecuter.RefreshExecuter();
            LoadComment();
            InputEditor.IsEnabled = true;
            InputEditor.Text = string.Empty;
            CloseQuote();
            await CommentScrollView.ScrollToAsync(CommentStack, ScrollToPosition.End, false);
            await PopupAction.DisplayPopup(new TipPopup("���۳ɹ�", 1));
        }
        catch (Exception ex)
        {
            SendButton.BackgroundColor = Color.FromArgb("#512BD4");
            InputEditor.IsEnabled = true;
            SendButton.IsEnabled = true;
            await PopupAction.DisplayPopup(new ErrorPopup("����ʧ��", $"������������\n{ex.Message}"));
            await Console.Out.WriteLineAsync(ex.ToString());
            return;
        }
    }

    private async void LikeButton_Clicked(object sender, EventArgs e)
    {
        if (!UserInfo.IsLoginSuccessful)
        {
            if (await DisplayAlert("����", "���ȵ�¼Ŷ", "ǰ����¼", "ȡ��"))
            {
                await Shell.Current.GoToAsync(nameof(UserLoginPage));
            }
            return;
        }
        if (LikeButton.IsLike)
        {
            try
            {
                CurrentPostData.PostLike++;
                LikeCount++;
                UserInfo.CurrentUser.LikedPostID += new string[] { CurrentPostData.PostID }.ToXFEString();
                await UserInfo.UpLoadUserInfo();
                if (await CurrentPostData.ExecuteUpdate(XFEExecuter) == 0)
                {
                    CurrentPostData.PostLike--;
                    LikeButton.IsLike = false;
                    await DisplayAlert("����ʧ��", "������������", "ȷ��");
                }
            }
            catch (Exception ex)
            {
                CurrentPostData.PostLike--;
                LikeButton.IsLike = false;
                await DisplayAlert("����ʧ��", "������������" + ex.Message, "ȷ��");
            }
        }
        else
        {
            try
            {
                if (CurrentPostData.PostLike >= 0)
                {
                    CurrentPostData.PostLike--;
                    LikeCount--;
                }
                UserInfo.CurrentUser.LikedPostID = UserInfo.CurrentUser.LikedPostID.Replace($"[+-{CurrentPostData.PostID}-+]", string.Empty);
                await UserInfo.UpLoadUserInfo();
                if (await CurrentPostData.ExecuteUpdate(XFEExecuter) == 0)
                {
                    CurrentPostData.PostLike++;
                    LikeCount++;
                    LikeButton.IsLike = true;
                    await DisplayAlert("ȡ������ʧ��", "������������", "ȷ��");
                }
            }
            catch (Exception ex)
            {
                CurrentPostData.PostLike++;
                LikeCount++;
                LikeButton.IsLike = true;
                await DisplayAlert("ȡ������ʧ��", "������������" + ex.Message, "ȷ��");
            }
        }
    }

    private async void StarButton_Clicked(object sender, EventArgs e)
    {
        if (!UserInfo.IsLoginSuccessful)
        {
            if (await DisplayAlert("����", "���ȵ�¼Ŷ", "ǰ����¼", "ȡ��"))
            {
                await Shell.Current.GoToAsync(nameof(UserLoginPage));
            }
            return;
        }
        if (StarButton.IsStar)
        {
            try
            {
                CurrentPostData.PostStar++;
                StarCount++;
                UserInfo.CurrentUser.StarredPostID += new string[] { CurrentPostData.PostID }.ToXFEString();
                await UserInfo.UpLoadUserInfo();
                if (await CurrentPostData.ExecuteUpdate(XFEExecuter) == 0)
                {
                    CurrentPostData.PostStar--;
                    StarButton.IsStar = false;
                    await DisplayAlert("�ղ�ʧ��", "������������", "ȷ��");
                }
            }
            catch (Exception ex)
            {
                CurrentPostData.PostStar--;
                StarButton.IsStar = false;
                await DisplayAlert("�ղ�ʧ��", "������������" + ex.Message, "ȷ��");
            }
        }
        else
        {
            try
            {
                if (CurrentPostData.PostStar >= 0)
                {
                    CurrentPostData.PostStar--;
                    StarCount--;
                }
                UserInfo.CurrentUser.StarredPostID = UserInfo.CurrentUser.StarredPostID.Replace($"[+-{CurrentPostData.PostID}-+]", string.Empty);
                await UserInfo.UpLoadUserInfo();
                if (await CurrentPostData.ExecuteUpdate(XFEExecuter) == 0)
                {
                    CurrentPostData.PostStar++;
                    StarCount++;
                    StarButton.IsStar = true;
                    await DisplayAlert("ȡ���ղ�ʧ��", "������������", "ȷ��");
                }
            }
            catch (Exception ex)
            {
                CurrentPostData.PostStar++;
                StarCount++;
                StarButton.IsStar = true;
                await DisplayAlert("ȡ���ղ�ʧ��", "������������" + ex.Message, "ȷ��");
            }
        }
    }

    private void QuoteCloseButton_Clicked(object sender, EventArgs e)
    {
        CloseQuote();
    }
    private void CloseQuote()
    {
        QuoteID = string.Empty;
        QuoteBorder.IsVisible = false;
    }
}
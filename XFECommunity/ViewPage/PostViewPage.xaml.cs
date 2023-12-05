using XFECommunity.AllImpl;
using XFECommunity.Controls;
using XFE各类拓展.NetCore.ArrayExtension;
using XFE各类拓展.NetCore.TaskExtension;
using XFE各类拓展.NetCore.XFEDataBase;

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
    private string? postID;

    public required string PostID
    {
        get { return postID is null ? string.Empty : postID; }
        set
        {
            if (string.IsNullOrEmpty(value))
                return;
            postID = value;
            if (!Initialized)
                InitializePageData();
        }
    }
    public XFEChatRoom_CommunityPost? CurrentPostData { get; set; }
    public static PostViewPage? Current { get; private set; }
    private readonly List<string> commentIDList = [];
    private readonly List<CommentCardView> commentCardList = [];
    private bool Editing = false;
    private bool Initialized = false;
    private bool RefreshingIsBusy = false;
    private long totalHeight = 0;
    private string QuoteID = string.Empty;
    private readonly XFEExecuter XFEExecuter = XCCDataBase.XFEDataBase!.CreateExecuter();
    public PostViewPage()
    {
        InitializeComponent();
        this.BindingContext = this;
        Current = this;
        backButton.Command = new Command(() =>
        {
            SendBackButtonPressed();
        });
    }

    protected override bool OnBackButtonPressed()
    {
        try { XFEExecuter.Dispose(); } catch (Exception ex) { Console.WriteLine(ex); }
        CurrentPostData = null;
#if WINDOWS
        Shell.Current.GoToAsync("..");
#endif
        return base.OnBackButtonPressed();
    }

    private async void InitializePageData()
    {
        Initialized = true;
        await Refresh();
        if (CurrentPostData!.UID == UserInfo.CurrentUser!.ID)
        {
            ToolbarItems.Add(new ToolbarItem
            {
                Text = "编辑",
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
                await DisplayAlert("哦不", "无法获取帖子信息\n帖子ID：" + PostID, "啊？");
                return;
            }
            this.Title = "小窝：" + (CurrentPostData.PostTitle!.Length > 10 ? CurrentPostData.PostTitle[..10] + "..." : CurrentPostData.PostTitle);
            TitleLabel.Text = CurrentPostData.PostTitle;
            charLabel.Text = CurrentPostData.UName![0].ToString();
            ContentLabel.Text = CurrentPostData.PostContent;
            AuthorLabel.Text = CurrentPostData.UName;
            TimeLabel.Text = CurrentPostData.PostTime.ToString();
            LikeCount = CurrentPostData.PostLike;
            StarCount = CurrentPostData.PostStar;
            if (UserInfo.IsLoginSuccessful)
            {
                LikeButton.IsLike = UserInfo.CurrentUser!.LikedPostID!.Contains(CurrentPostData.PostID!);
                StarButton.IsStar = UserInfo.CurrentUser!.StarredPostID!.Contains(CurrentPostData.PostID!);
            }
            tagStackLayout.Clear();
            foreach (var tag in CurrentPostData.PostTag!.ToXFEArray<string>())
            {
                var button = new Button
                {
                    Text = $"#{tag}",
                    CornerRadius = 20,
                    Margin = new Thickness(5, 3, 0, 3)
                };
                button.SetDynamicResource(Button.TextColorProperty, "MainColor");
                button.SetDynamicResource(Button.BackgroundColorProperty, "LightMainColor");
                button.Clicked += TagButton_Clicked;
                tagStackLayout.Children.Add(button);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("哦不", "无法获取帖子信息\n帖子ID：" + PostID + "\n" + ex.Message, "啊？");
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
            using (var executer = XCCDataBase.XFEDataBase!.CreateExecuter())
            {
                List<XFEChatRoom_CommunityComment>? commentDataList = [];
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
                            XFEChatRoom_CommunityComment? quoteCommentData = null;
                            while (getQuoteRetryTime < 30)
                            {
                                try
                                {
                                    quoteCommentData = await executer.ExecuteGetFirst<XFEChatRoom_CommunityComment>(x => x.CommentID == commentData.QuoteID);
                                    break;
                                }
                                catch (Exception)
                                {
                                    if (getQuoteRetryTime == 29)
                                    {
                                        quoteCommentData = null;
                                        await DisplayAlert("加载失败", $"无法获取评论的引用列表", "确认");
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
                                    if (UserInfo.IsLoginSuccessful && UserInfo.CurrentUser!.LikedCommentID!.Contains(tarCommentCard.CommentID))
                                    {
                                        tarCommentCard.IsLike = true;
                                    }
                                });
                            }
                            else
                            {
                                var commentCard = new CommentCardView(commentData, quoteCommentData)
                                {
                                    Margin = new Thickness(0, 5)
                                };
                                commentCard.LikeClick += CommentCard_LikeClick;
                                commentCard.QuoteClick += CommentCard_QuoteClick;
                                commentCard.CommentCardTapped += CommentCard_CommentCardTapped;
                                if (UserInfo.IsLoginSuccessful && UserInfo.CurrentUser!.LikedCommentID!.Contains(commentData.CommentID!))
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
                    await DisplayAlert("加载失败", $"无法获取评论列表", "确认");
                }
            }
            RefreshingIsBusy = false;
        }).StartNewTask();
    }

    private void CommentCard_CommentCardTapped(object? sender, EventArgs e)
    {
        if (sender is CommentCardView commentCard)
        {
            SwitchToQuoteStyle(commentCard.CommentID);
            InputEditor.Focus();
        }
    }

    private void CommentCard_QuoteClick(object? sender, CommentCardQuoteClickEventArgs e)
    {
        if (sender is CommentCardView commentCard)
        {
            SwitchToQuoteStyle(commentCard.CommentID);
            InputEditor.Focus();
        }
    }

    private async void CommentCard_LikeClick(object? sender, CommentCardLikeClickEventArgs e)
    {
        if (!UserInfo.IsLoginSuccessful)
        {
            if (await DisplayAlert("诶嘿", "请先登录哦", "前往登录", "取消"))
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
                    UserInfo.CurrentUser!.LikedCommentID += new string[] { commentCard.CommentID }.ToXFEString();
                    await UserInfo.UpLoadUserInfo();
                    if (await commentCard.CurrentCommentData.ExecuteUpdate(XFEExecuter) == 0)
                    {
                        commentCard.IsLike = false;
                        await DisplayAlert("点赞失败", "请检查网络设置", "确定");
                    }
                }
                catch (Exception ex)
                {
                    commentCard.IsLike = false;
                    await DisplayAlert("点赞失败", "请检查网络设置" + ex.Message, "确定");
                }
            }
            else
            {
                try
                {
                    UserInfo.CurrentUser!.LikedCommentID = UserInfo.CurrentUser!.LikedCommentID!.Replace($"[+-{commentCard.CommentID}-+]", string.Empty);
                    await UserInfo.UpLoadUserInfo();
                    if (await commentCard.CurrentCommentData.ExecuteUpdate(XFEExecuter) == 0)
                    {
                        commentCard.IsLike = true;
                        await DisplayAlert("取消点赞失败", "请检查网络设置", "确定");
                    }
                }
                catch (Exception ex)
                {
                    commentCard.IsLike = true;
                    await DisplayAlert("取消点赞失败", "请检查网络设置" + ex.Message, "确定");
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

    private async void TagButton_Clicked(object? sender, EventArgs e)
    {
        if (sender is Button button)
            await DisplayAlert("标签", $"{button.Text[1..]}", "确定");
    }

    private void InputEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(InputEditor.Text))
        {
            SendButton.SetDynamicResource(ImageButton.BackgroundColorProperty, "DisabledMainColor");
            SendButton.IsEnabled = false;
        }
        else
        {
            SendButton.SetDynamicResource(ImageButton.BackgroundColorProperty, "MainColor");
            SendButton.IsEnabled = true;
        }
    }

    private void SwitchToQuoteStyle(string quoteID)
    {
        QuoteID = quoteID;
        var tarComment = commentCardList.Find(x => x.CommentID == quoteID);
        QuoteLabel.Text = $"{tarComment!.UserName}：{tarComment.CommentContent}";
        QuoteBorder.IsVisible = true;
    }

    private async void SendButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (!UserInfo.IsLoginSuccessful)
            {
                if (await DisplayAlert("诶嘿", "请先登录哦", "前往登录", "取消"))
                {
                    await Shell.Current.GoToAsync(nameof(UserLoginPage));
                }
                return;
            }
            SendButton.IsEnabled = false;
            SendButton.SetDynamicResource(BackgroundColorProperty, "DisabledMainColor");
            InputEditor.IsEnabled = false;
            var tarCommentId = await IDGenerator.GetCorrectCommentId(XFEExecuter);
            try
            {
                var result = await XFEExecuter.ExecuteAdd(new XFEChatRoom_CommunityComment
                {
                    CommentID = tarCommentId,
                    PostID = PostID,
                    CommentContent = InputEditor.Text,
                    UID = UserInfo.CurrentUser!.ID,
                    UName = UserInfo.CurrentUser.Aname,
                    FloorCount = await AppAlgorithm.GetCommentFloor(PostID, XFEExecuter),
                    QuoteID = QuoteID
                });
                if (result == 0)
                {
                    SendButton.SetDynamicResource(BackgroundColorProperty, "MainColor");
                    InputEditor.IsEnabled = true;
                    SendButton.IsEnabled = true;
                    await DisplayAlert("评论失败", "请检查网络设置", "确认");
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
        }
        catch (Exception ex)
        {
            SendButton.SetDynamicResource(BackgroundColorProperty, "MainColor");
            InputEditor.IsEnabled = true;
            SendButton.IsEnabled = true;
            await DisplayAlert("评论失败", $"请检查网络设置\n{ex.Message}", "确认");
            await Console.Out.WriteLineAsync(ex.ToString());
            return;
        }
    }

    private async void LikeButton_Clicked(object sender, EventArgs e)
    {
        if (!UserInfo.IsLoginSuccessful)
        {
            if (await DisplayAlert("诶嘿", "请先登录哦", "前往登录", "取消"))
            {
                await Shell.Current.GoToAsync(nameof(UserLoginPage));
            }
            return;
        }
        if (LikeButton.IsLike)
        {
            try
            {
                CurrentPostData!.PostLike++;
                LikeCount++;
                UserInfo.CurrentUser!.LikedPostID += new string[] { CurrentPostData.PostID! }.ToXFEString();
                await UserInfo.UpLoadUserInfo();
                if (await CurrentPostData.ExecuteUpdate(XFEExecuter) == 0)
                {
                    CurrentPostData.PostLike--;
                    LikeButton.IsLike = false;
                    await DisplayAlert("点赞失败", "请检查网络设置", "确定");
                }
            }
            catch (Exception ex)
            {
                CurrentPostData!.PostLike--;
                LikeButton.IsLike = false;
                await DisplayAlert("点赞失败", "请检查网络设置" + ex.Message, "确定");
            }
        }
        else
        {
            try
            {
                if (CurrentPostData!.PostLike >= 0)
                {
                    CurrentPostData.PostLike--;
                    LikeCount--;
                }
                UserInfo.CurrentUser!.LikedPostID = UserInfo.CurrentUser.LikedPostID!.Replace($"[+-{CurrentPostData.PostID}-+]", string.Empty);
                await UserInfo.UpLoadUserInfo();
                if (await CurrentPostData.ExecuteUpdate(XFEExecuter) == 0)
                {
                    CurrentPostData.PostLike++;
                    LikeCount++;
                    LikeButton.IsLike = true;
                    await DisplayAlert("取消点赞失败", "请检查网络设置", "确定");
                }
            }
            catch (Exception ex)
            {
                CurrentPostData!.PostLike++;
                LikeCount++;
                LikeButton.IsLike = true;
                await DisplayAlert("取消点赞失败", "请检查网络设置" + ex.Message, "确定");
            }
        }
    }

    private async void StarButton_Clicked(object sender, EventArgs e)
    {
        if (!UserInfo.IsLoginSuccessful)
        {
            if (await DisplayAlert("诶嘿", "请先登录哦", "前往登录", "取消"))
            {
                await Shell.Current.GoToAsync(nameof(UserLoginPage));
            }
            return;
        }
        if (StarButton.IsStar)
        {
            try
            {
                CurrentPostData!.PostStar++;
                StarCount++;
                UserInfo.CurrentUser!.StarredPostID += new string[] { CurrentPostData.PostID! }.ToXFEString();
                await UserInfo.UpLoadUserInfo();
                if (await CurrentPostData.ExecuteUpdate(XFEExecuter) == 0)
                {
                    CurrentPostData.PostStar--;
                    StarButton.IsStar = false;
                    await DisplayAlert("收藏失败", "请检查网络设置", "确定");
                }
            }
            catch (Exception ex)
            {
                CurrentPostData!.PostStar--;
                StarButton.IsStar = false;
                await DisplayAlert("收藏失败", "请检查网络设置" + ex.Message, "确定");
            }
        }
        else
        {
            try
            {
                if (CurrentPostData!.PostStar >= 0)
                {
                    CurrentPostData.PostStar--;
                    StarCount--;
                }
                UserInfo.CurrentUser!.StarredPostID = UserInfo.CurrentUser.StarredPostID!.Replace($"[+-{CurrentPostData.PostID}-+]", string.Empty);
                await UserInfo.UpLoadUserInfo();
                if (await CurrentPostData.ExecuteUpdate(XFEExecuter) == 0)
                {
                    CurrentPostData.PostStar++;
                    StarCount++;
                    StarButton.IsStar = true;
                    await DisplayAlert("取消收藏失败", "请检查网络设置", "确定");
                }
            }
            catch (Exception ex)
            {
                CurrentPostData!.PostStar++;
                StarCount++;
                StarButton.IsStar = true;
                await DisplayAlert("取消收藏失败", "请检查网络设置" + ex.Message, "确定");
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
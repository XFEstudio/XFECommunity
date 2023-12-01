﻿namespace XCCChatRoom.Controls
{
    public class WaitableButton : Button
    {
        private bool isWaiting;

        public bool IsWaiting
        {
            get { return isWaiting; }
            set
            {
                isWaiting = value;
                if (value)
                {
                    this.IsEnabled = false;
                    if (UseLoadingAnimation)
                        this.ImageSource = "whiteloader";
                    TextColor ??= Color.Parse("White");
                    BackgroundColor ??= Color.FromArgb("#512BD4");
                    this.BackgroundColor = this.BackgroundColor.MultiplyAlpha(0.5f);
                }
                else
                {
                    this.BackgroundColor = this.BackgroundColor.MultiplyAlpha(2f);
                    if (UseLoadingAnimation)
                        this.ImageSource = null;
                    this.IsEnabled = true;
                }
            }
        }

        public bool UseLoadingAnimation { get; set; } = true;
        public event EventHandler<WaitButtonClickedEventArgs> WaitClick;
        private void WaitButton_Clicked(object sender, EventArgs e)
        {
            IsWaiting = true;
            this.WaitClick?.Invoke(this, new WaitButtonClickedEventArgs(e, this));
        }
        public WaitableButton()
        {
            Clicked += WaitButton_Clicked;
        }
    }
    public class WaitButtonClickedEventArgs(EventArgs e, WaitableButton button) : EventArgs
    {
        public EventArgs ClickedEventArgs { get; private set; } = e;
        private readonly WaitableButton button = button;
        public void Continue()
        {
            button.Dispatcher.Dispatch(() =>
            {
                button.IsWaiting = false;
            });
        }
    }
}

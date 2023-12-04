namespace XFECommunity.AllImpl
{
    public static class ProcessException
    {
        public static async Task ShowExceptionBase(string title, string noExceptionTip, Exception? exception) => await Shell.Current.DisplayAlert(title, exception is null ? noExceptionTip : $"详细错误：{exception.Message}", "确认");
        public static async Task ShowEnumException(Exception? exception = null) => await ShowExceptionBase("枚举异常", "发生枚举错误", exception);
        public static async Task ShowDataBaseException(Exception? exception = null) => await ShowExceptionBase("数据库异常", "数据库发生错误", exception);
        public static async Task ShowDataBaseExecuteFailException(Exception? exception = null) => await ShowExceptionBase("数据库执行失败", "执行返回结果为0", exception);
        public static async Task ShowNetWorkException(Exception? exception = null) => await ShowExceptionBase("网络错误", "网络连接错误", exception);
    }
}

namespace XFECommunity.AllImpl
{
    public static class AppPath
    {
        public static string UserInfoPath { get; } = $"{FileSystem.Current.CacheDirectory}/UserInfo.xfe";
        public static string SystemProfilePath { get; } = $"{FileSystem.Current.CacheDirectory}/SystemProfile.xfe";
        public static string GPTAIDialogsPath { get; } = $"{FileSystem.Current.CacheDirectory}/GPTAIDialogs.xfe";
        public static string ChatDialogHistoryPath { get; } = $"{FileSystem.Current.CacheDirectory}/ChatDialogHistory";
        public static string CheckInitializePath { get; } = $"{FileSystem.Current.AppDataDirectory}/Initialize.xfe";
    }
}

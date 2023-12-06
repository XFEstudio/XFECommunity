namespace XFECommunity.AllImpl
{
    public static class StringVerify
    {
        public static string[] ContrabandVocabulary { get; } = ["脏话1", "脏话2", "脏话3"];
        public static bool VerifyString(this string verifyString)
        {
            int i = 1;
            foreach (string word in ContrabandVocabulary)
            {
                if (verifyString == word)
                {
                    i = -1;
                    break;
                }
            }
            if (i > 0)
                return true;
            else
                return false;
        }

        public static bool VerifyUserName(this string newUserName)
        {
            if (!newUserName.Contains(' ') && newUserName.VerifyString()) { return true; }
            else { return false; }
        }

        public static bool VerifyPassword(this string newPassword)
        {
            if (newPassword is not null && newPassword != string.Empty)
            {
                if (!newPassword.Contains(' ') && newPassword.Length < 20) { return true; }
                else { return false; }
            }
            else { return false; }
        }
    }
}

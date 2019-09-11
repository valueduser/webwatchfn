namespace WebWatcher
{
    public class Constants
    {
        #region html
        public const string beginDiv = "<div>";
        public const string endDiv = "</div>";
        public const string endSpan = "</span>";
        public const string lightGreen = "#ccffcc";
        public const string lightRed = "#ffcccc";
        public const string nonBreakingSpace = "\x00a0";
        #endregion
        public static string[] IgnoredDiffStrings = { "nonce", "postnonce" };
    }
}
namespace PrintBed.Helpers
{
    public static class SafeString
    {
        public static string Convert(string str)
        {
            str = str.Trim();
            str = str.ToLower();
            str = str.Replace(' ', '-');

            List<char> invalidFileNameChars = Path.GetInvalidFileNameChars().ToList<char>();
            invalidFileNameChars.AddRange("!*'();:@&=+$,/?%#[]");
            // Builds a string out of valid chars and an _ for invalid ones
            var safeStr = new string(str.Select(ch => invalidFileNameChars.Contains(ch) ? '_' : ch).ToArray());
            return safeStr;
        }
    }
}

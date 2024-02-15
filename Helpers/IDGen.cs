using System.Text;

namespace PrintBed.Helpers
{
    public static class IDGen
    {
        private static char[] _base62chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();

        private static Random _random = new Random();

        public static string GetBase62(int length)
        {
            var sb = new StringBuilder(length);

            for (int i = 0; i < length; i++)
                sb.Append(_base62chars[_random.Next(62)]);

            return sb.ToString();
        }

        public static string GetBase36(int length)
        {
            var sb = new StringBuilder(length);

            for (int i = 0; i < length; i++)
                sb.Append(_base62chars[_random.Next(36)]);

            return sb.ToString();
        }

        public static string GetBase64(string source)
        {
            var textBytes = Encoding.UTF8.GetBytes(source);
            var base64String = System.Convert.ToBase64String(textBytes);
            return base64String;
        }
    }

}

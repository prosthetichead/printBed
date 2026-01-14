using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;

namespace PrintBed.Helpers
{
    public static class IdGen
    {
        // Define the alphabet as a constant string
        private const string Base62Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        public static string GetBase62(int length)
        {
            return RandomNumberGenerator.GetString(Base62Chars, length);
        }

        public static string GetBase36(int length)
        {
            return RandomNumberGenerator.GetString(Base62Chars.AsSpan(0, 36), length);
        }

    }

}

namespace PrintBed.Extensions
{
    public static class FileExtensions
    {
        public static string ToPrettySize(this double fileSize)
        {
            if (fileSize <= 0) return "0 B";
            double len = fileSize;
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1) { order++; len = len / 1024; }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}

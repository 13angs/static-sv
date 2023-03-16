namespace static_sv.DTOs
{
    public static class StaticTypes
    {
        public static string Image = "image";
        public static string Video = "video";
        public static string Text = "text";
        public static string Application = "application";
        public static List<string> AllTypes = new List<string>{
            Image,
            Video,
            Text,
            Application
        };
    }
}
namespace BoulderBuddy.Utility
{
    public static class ImageUtility
    {
        public enum ImageType
        {
            Unknown,
            Preview
        }

        public static string GetImagePath(IWebHostEnvironment env, string imageName, ImageType imageType)
        {
            string placeholderPath = @"\res\route_previews\placeholder.jpg";
            string result = "";
            string path = "";

            if (imageType == ImageType.Preview)
            {
                path = @"\res\route_previews\";
            }

            result = (!System.IO.File.Exists(Path.Combine(env.WebRootPath, path.Substring(1) + imageName)) ? placeholderPath : path + imageName);

            return result;
        }
    }
}

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace BoulderBuddy.Utility
{
    public static class ImageUtility
    {
        public static string PreviewPath = @"\res\route_previews\";
        public static string PlaceholderPath = @"\res\route_previews\placeholder.jpg";
        public static string UserImagePath = @"\res\user_images\";

        public static bool RemovePreview(IWebHostEnvironment env, string imageName)
        {
            string path = GetFullPath(env, GetPreviewOrPlaceholder(env, imageName));

            if (!Path.Exists(path))
            {
                return false;
            }
            else if (Path.Equals(path, GetFullPath(env, PlaceholderPath)))
            {
                return false;
            }

            System.IO.File.Delete(path);

            return true;
        }

        public static bool RemoveUserImage(IWebHostEnvironment env, string imageName)
        {
            string path = GetFullPath(env, GetUserImageOrPlaceholder(env, imageName));

            if (!Path.Exists(path))
            {
                return false;
            }
            else if (Path.Equals(path, GetFullPath(env, PlaceholderPath)))
            {
                return false;
            }

            System.IO.File.Delete(path);

            return true;
        }

        public static string GetPreviewOrPlaceholder(IWebHostEnvironment env, string imageName)
        {
            string pathExpected = "";

            if (string.IsNullOrWhiteSpace(imageName))
            {
                return PlaceholderPath;
            }

            pathExpected = System.IO.Path.Join(PreviewPath + imageName);

            return (!Path.Exists(GetFullPath(env, pathExpected)) ? PlaceholderPath : pathExpected);
        }

        public static string GetUserImageOrPlaceholder(IWebHostEnvironment env, string imageName)
        {
            string pathExpected = "";

            if (string.IsNullOrWhiteSpace(imageName))
            {
                return PlaceholderPath;
            }

            pathExpected = System.IO.Path.Join(UserImagePath + imageName);

            return (!Path.Exists(GetFullPath(env, pathExpected)) ? PlaceholderPath : pathExpected);
        }

        public static string CreateNewPreviewPath(IWebHostEnvironment env, IFormFile? previewImage)
        {
            if (previewImage == null) return "";
            if (string.IsNullOrWhiteSpace(previewImage.FileName)) return "";

            string fileName = Guid.NewGuid() + Path.GetExtension(previewImage?.FileName);

            string newPath = System.IO.Path.Join(PreviewPath + fileName);

            return newPath;
        }

        public static string CreateNewUserImagePath(IWebHostEnvironment env, IFormFile? userImage)
        {
            if (userImage == null) return "";
            if (string.IsNullOrWhiteSpace(userImage.FileName)) return "";

            string fileName = Guid.NewGuid() + Path.GetExtension(userImage?.FileName);

            string newPath = System.IO.Path.Join(UserImagePath + fileName);

            return newPath;
        }

        public static string GetFullPath(IWebHostEnvironment env, string path)
        {
            return System.IO.Path.Join(env.WebRootPath, path);
        }
    }
}

namespace InvestmentApi.Handlers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S125:Sections of code should not be commented out", Justification = "<Pending>")]
    public static class FileHandler
    {
        public static string? Upload(IFormFile? file, string id, string folderName, bool isEditMode = false, string? imagePath = "")
        {
            if (isEditMode && file == null)
            {
                return imagePath;
            }

            if (file == null)
            {
                return string.Empty;
            }

            string basePath = $"Images/{folderName}/{id}";
            string path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/{basePath}");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            Guid guid = Guid.NewGuid();
            FileInfo fileInfo = new(file.FileName);
            string fileName = guid + fileInfo.Extension;

            string fullFilePath = Path.Combine(path, fileName);           

            using (var stream = new FileStream(fullFilePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            if (isEditMode && !string.IsNullOrWhiteSpace(imagePath))
            {
                string[] oldImagePart = imagePath.Split("/");
                string oldFileName = oldImagePart[^1];
                string completeOldPath = Path.Combine(path, oldFileName);

                if (File.Exists(completeOldPath))
                {
                    File.Delete(completeOldPath);
                }
            }

            return $"{basePath}/{fileName}";
        }

        public static bool Delete(string id, string folderName)
        {
            string basePath = $"Images/{folderName}/{id}";
            string path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/{basePath}");

            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            else
            {
                return false;
            }

            return true;
        }       
    }
}

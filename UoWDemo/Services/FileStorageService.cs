namespace UsersManagement.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _imagesFolderPath;

        public FileStorageService(IWebHostEnvironment webHostEnvironment)
        {
            _imagesFolderPath = Path.Combine(webHostEnvironment.ContentRootPath, "images");
        }

        public async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                throw new ArgumentException("The image file is invalid.", nameof(imageFile));
            }


            if (!Directory.Exists(_imagesFolderPath))
            {
                Directory.CreateDirectory(_imagesFolderPath);
            }


            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(_imagesFolderPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return Path.Combine("images", uniqueFileName);
        }
        public async Task DeleteImageAsync(string imagePath)
        {
            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
            }
        }

    }

}

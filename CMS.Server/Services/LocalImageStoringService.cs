namespace CMS.Server.Services
{
    public class LocalImageStorageService : IImageStorageService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LocalImageStorageService(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty");

            // Create image folder if it doesn't exist
            var folderPath = Path.Combine(_env.WebRootPath, "images");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            // Generate a unique filename
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var fullPath = Path.Combine(folderPath, fileName);

            // Save the file
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Get the base URL (http/https and port)
            var request = _httpContextAccessor.HttpContext?.Request;
            if (request == null)
                throw new InvalidOperationException("Unable to access the current HTTP context.");

            var baseUrl = $"{request.Scheme}://{request.Host}";

            // Return the full URL of the image
            return $"{baseUrl}/images/{fileName}";
        }

        public async Task DeleteImageAsync(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;

            try
            {
                // Extract the filename from the URL
                Uri uri = new Uri(imageUrl);
                string relativePath = uri.AbsolutePath;  // This will get the path after the domain, like "/images/filename.jpg"

                // Combine with web root path to get the full physical file path
                string filePath = Path.Combine(_env.WebRootPath, relativePath.TrimStart('/'));

                Console.WriteLine($"Attempting to delete file at: {filePath}");

                if (File.Exists(filePath))
                {
                    await Task.Run(() => File.Delete(filePath));
                    Console.WriteLine($"Successfully deleted file: {filePath}");
                }
                else
                {
                    Console.WriteLine($"File not found at path: {filePath}");
                }
            }
            catch (Exception ex)
            {
                // Log the error but don't throw it to prevent disrupting the application flow
                Console.WriteLine($"Error deleting image file: {ex.Message}");
            }
        }

    }
}
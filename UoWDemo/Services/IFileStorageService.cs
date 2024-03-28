namespace UsersManagement.Services
{
    public interface IFileStorageService
    {
        Task<string> SaveImageAsync(IFormFile imageFile);
        Task DeleteImageAsync(string imagePath);

    }

}

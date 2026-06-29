using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MedicalForum.WebApi.Application.Interfaces
{
    public interface IFileStorageService
    {
        /// <summary>
        /// Uploads a file and returns its public URL/path.
        /// </summary>
        Task<string> UploadFileAsync(IFormFile file, string folderName);
        
        /// <summary>
        /// Deletes a file from storage.
        /// </summary>
        Task DeleteFileAsync(string fileUrl);
    }
}

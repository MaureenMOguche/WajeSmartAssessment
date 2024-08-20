using Microsoft.AspNetCore.Http;

namespace WajeSmartAssessment.Application.Contracts.Services;
public interface IFirebaseService
{
    Task<string> Upload(IFormFile file, string filename, string foldername);
    Task Delete(string fileName);
    Task<List<string>> UploadFiles(IFormFileCollection files, string foldername);
}


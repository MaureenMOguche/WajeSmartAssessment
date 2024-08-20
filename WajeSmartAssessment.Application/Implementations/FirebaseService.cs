using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using WajeSmartAssessment.Application.Contracts.Services;

namespace WajeSmartAssessment.Application.Implementations;

public class FirebaseService : IFirebaseService
{
    public async Task Delete(string fileName)
    {
        var task = new FirebaseStorage("wazobia-3b2dc.appspot.com")
            .Child("avatars")
            .Child(fileName)
            .DeleteAsync();

        await task;
    }

    public async Task<string> Upload(IFormFile file, string filename, string foldername)
    {
        var stream = file.OpenReadStream();
        var task = new FirebaseStorage("wazobia-3b2dc.appspot.com")
            .Child(foldername)
            .Child(filename)
            .PutAsync(stream);

        var downloadUrl = await task;
        return downloadUrl;
    }

    public async Task<List<string>> UploadFiles(IFormFileCollection files, string foldername)
    {
        List<string> uploadedMedia = [];
        foreach (var file in files)
        {
            var stream = file.OpenReadStream();
            var task = new FirebaseStorage("wazobia-3b2dc.appspot.com")
                .Child(foldername)
                .Child(Guid.NewGuid().ToString())
                .PutAsync(stream);

            var downloadUrl = await task;
            uploadedMedia.Add(downloadUrl);
        }
        return uploadedMedia;
    }
}

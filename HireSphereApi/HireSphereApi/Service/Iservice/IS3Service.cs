using Amazon.S3.Model;
using Amazon.S3;
using Microsoft.AspNetCore.Mvc;

namespace HireSphereApi.Service.Iservice
{
    public interface IS3Service
    {
         Task<string> GeneratePresignedUrlToUpload([FromQuery] string fileName);
         Task<string> GeneratePresignedUrlToDownload([FromQuery] string fileName);
        Task<string> GeneratePresignedUrlToView(string fileName);

        //Task<Stream> DownloadFileAsync(string s3Key);

    }
}

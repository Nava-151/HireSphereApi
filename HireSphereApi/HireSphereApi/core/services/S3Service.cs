namespace HireSphereApi.core.services
{
    using Amazon.S3;
    using Amazon.S3.Model;
    using HireSphereApi.Service.Iservice;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using System.IO;
    using System.Threading.Tasks;

    public class S3Service:IS3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public S3Service(IConfiguration config,IAmazonS3 s3client)
        {
            _s3Client = s3client;
            _bucketName = "hiresphere";
           
        }
      
        public async Task<string> GeneratePresignedUrlToDownload(string fileName)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = fileName,
                Expires = DateTime.UtcNow.AddMinutes(5), 
                Verb = HttpVerb.GET
            };

            return _s3Client.GetPreSignedURL(request);
        }
        public async Task<string> GeneratePresignedUrlToView(string fileName)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = fileName,
                Expires = DateTime.UtcNow.AddMinutes(5),
                Verb = HttpVerb.GET,
                ResponseHeaderOverrides = new ResponseHeaderOverrides
                {
                    ContentDisposition = "inline",
                }
            };

            return _s3Client.GetPreSignedURL(request);
        }

        public async Task<string> GeneratePresignedUrlToUpload(string fileName)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = fileName,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddMinutes(5),
            };
            string url = _s3Client.GetPreSignedURL(request);
            return url;
        }

    }
}

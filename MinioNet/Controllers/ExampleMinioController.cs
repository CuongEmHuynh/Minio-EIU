

using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using System;
using System.Net;
using System.Security.AccessControl;

namespace MinioNet.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExampleMinioController : ControllerBase
    {
        [HttpGet("Connect")]
        public async Task<IActionResult> ConnectMinio()
        {
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

            var endpoint = Environment.GetEnvironmentVariable("MINIO_ENDPOINT");
            var accessKey = Environment.GetEnvironmentVariable("MINIO_ACCESSKEY");
            var secretKey = Environment.GetEnvironmentVariable("MINIO_SCRETKEY");
            IMinioClient minio = new MinioClient()
                                    .WithEndpoint(endpoint)
            .WithCredentials(accessKey, secretKey)
                                    .WithSSL(false)
                                    .Build();

            // bool found = await minio.BucketExistsAsync("");
            //Console.WriteLine("bucket-name " + ((found == true) ? "exists" : "does not exist"));
            Console.WriteLine("Running example for API: MakeBucketAsync");

            Console.WriteLine($"Created bucket test");
            Console.WriteLine();

            //Get Bucket
            var getListBucketsTask = await minio.ListBucketsAsync().ConfigureAwait(false);

            //Create Bucket
            await minio.MakeBucketAsync(new MakeBucketArgs().WithBucket("test"));

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> UploadFileToMinio(IFormFile file)
        {

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

            var endpoint = Environment.GetEnvironmentVariable("MINIO_ENDPOINT");
            var accessKey = Environment.GetEnvironmentVariable("MINIO_ACCESSKEY");
            var secretKey = Environment.GetEnvironmentVariable("MINIO_SCRETKEY");
            IMinioClient minio = new MinioClient()
                                    .WithEndpoint(endpoint)
            .WithCredentials(accessKey, secretKey)
                                    .WithSSL(false)
                                    .Build();
            var fileTest = file.OpenReadStream();

            var bucketName = "target-version";
            var objectName = "/Info-Server.txt";
            var contentType = "application/zip";
            var filePath = "E:\\EIU\\Minio\\docker-compose.yml";
            try
            {
                // Make a bucket on the server, if not already present.
                var beArgs = new BucketExistsArgs()
                    .WithBucket(bucketName);
                bool found = await minio.BucketExistsAsync(beArgs).ConfigureAwait(false);
                if (!found)
                {
                    var mbArgs = new MakeBucketArgs()
                        .WithBucket(bucketName);
                    await minio.MakeBucketAsync(mbArgs).ConfigureAwait(false);
                }
                var fileName = "HCE/" + file.FileName;
                // Upload a file to bucket.
                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(fileName)
                    .WithStreamData(fileTest).WithObjectSize(fileTest.Length);

                await minio.PutObjectAsync(putObjectArgs).ConfigureAwait(false);

                Console.WriteLine("Successfully uploaded " + objectName);
            }
            catch (MinioException e)
            {
                Console.WriteLine("File Upload Error: {0}", e.Message);
            }

            return NoContent();
        }

        [HttpPost("GetFile")]
        public async Task<IActionResult> GetFileInMinio()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

            var endpoint = Environment.GetEnvironmentVariable("MINIO_ENDPOINT");
            var accessKey = Environment.GetEnvironmentVariable("MINIO_ACCESSKEY");
            var secretKey = Environment.GetEnvironmentVariable("MINIO_SCRETKEY");
            IMinioClient minio = new MinioClient()
                                    .WithEndpoint(endpoint)
            .WithCredentials(accessKey, secretKey)
                                    .WithSSL(false)
                                    .Build();
            string bucketName = "target-version";
            string fileName = "Report-Car.xlsx";
            try
            {
                StatObjectArgs statObjectArgs = new StatObjectArgs()
                                    .WithBucket(bucketName)
                                    .WithObject(fileName);
                await minio.StatObjectAsync(statObjectArgs);

                var memoryStream = new MemoryStream();

                // Get input stream to have content of 'my-objectname' from 'my-bucketname'
                GetObjectArgs getObjectArgs = new GetObjectArgs()
                                                  .WithBucket(bucketName)
                                                  .WithObject(fileName)
                                                  .WithCallbackStream((stream) =>
                                                  {
                                                      stream.CopyTo(memoryStream);
                                                  });
                await minio.GetObjectAsync(getObjectArgs);
                memoryStream.Position = 0;

                // Trả file về client
                return File(memoryStream, "application/octet-stream", fileName);
            }
            catch (MinioException e)
            {
                Console.WriteLine("File Upload Error: {0}", e.Message);
                return StatusCode(500, "Error downloading file");
            }
        }

        
        [HttpGet("DeleteFile")]
        public async Task<IActionResult> DeleteFileInMinio()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

            var endpoint = Environment.GetEnvironmentVariable("MINIO_ENDPOINT");
            var accessKey = Environment.GetEnvironmentVariable("MINIO_ACCESSKEY");
            var secretKey = Environment.GetEnvironmentVariable("MINIO_SCRETKEY");
            IMinioClient minio = new MinioClient()
                                    .WithEndpoint(endpoint)
            .WithCredentials(accessKey, secretKey)
                                    .WithSSL(false)
                                    .Build();
            string bucketName = "target-version";
            string objectName = "HCE/MATS.txt";

            try
            {
                var args = new RemoveObjectArgs()
                     .WithBucket(bucketName)
                     .WithObject(objectName);
              

                Console.WriteLine("Running example for API: RemoveObjectAsync");
                await minio.RemoveObjectAsync(args).ConfigureAwait(false);
            }
            catch (Exception)
            {

                throw;
            }

            return NoContent();
        }
    }
}

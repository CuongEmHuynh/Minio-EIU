using Microsoft.Extensions.DependencyInjection;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using Minio.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MinioUtility
{
    public static class MinioUtility
    {
        private static IMinioClient _minioClient;
        private static string _bucketName;

        /// <summary>
        /// Init minio with 
        /// </summary>
        /// <returns></returns>
        public static async Task<IMinioClient> CreateMinioClient()
        {
            // Lấy thông tin từ biến môi trường
            var endpoint = Environment.GetEnvironmentVariable("MINIO_ENDPOINT");
            var accessKey = Environment.GetEnvironmentVariable("MINIO_ACCESSKEY");
            var secretKey = Environment.GetEnvironmentVariable("MINIO_SECRETKEY");
            var bucketName = Environment.GetEnvironmentVariable("BUCKET_NAME");
            _bucketName = bucketName;
            // Tạo đối tượng MinioClient
            _minioClient = new MinioClient()
                                       .WithEndpoint(endpoint)
                                       .WithCredentials(accessKey, secretKey)
                                       .WithSSL(false)
                                       .Build();
            var bucketExistsArgs = new BucketExistsArgs().WithBucket(bucketName);
            bool found = await _minioClient.BucketExistsAsync(bucketExistsArgs).ConfigureAwait(false);

            // Nếu bucket không tồn tại, tạo bucket mới
            if (!found)
            {
                var makeBucketArgs = new MakeBucketArgs().WithBucket(bucketName);
                await _minioClient.MakeBucketAsync(makeBucketArgs).ConfigureAwait(false);
                Console.WriteLine($"Bucket '{bucketName}' created successfully.");
            }
            else
            {
                Console.WriteLine($"Bucket '{bucketName}' already exists.");
            }

            return _minioClient;
        }

        /// <summary>
        /// Upload file to minio
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="objectName"></param>
        /// <param name="filePath"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static async Task UploadFileAsync(string objectName, string filePath, string contentType , string Folder = "FileUpload")
        {
            try
            {
                // Upload file lên bucket
                var putObjectArgs = new PutObjectArgs()
                                    .WithBucket(_bucketName)
                                    .WithObject(objectName)
                                    .WithFileName(filePath)
                                    .WithContentType(contentType);

                await _minioClient.PutObjectAsync(putObjectArgs).ConfigureAwait(false);
                Console.WriteLine($"Successfully uploaded {objectName} to bucket {_bucketName}");
            }
            catch (MinioException e)
            {
                Console.WriteLine($"File Upload Error: {e.Message}");
            }
        }

        public static async Task DownloadFile(string fileNameDB = "my-object-name")
        {
            StatObjectArgs statObjectArgs = new StatObjectArgs().WithBucket(_bucketName).WithObject(fileNameDB);
            await _minioClient.StatObjectAsync(statObjectArgs).ConfigureAwait(false);

            var args = new GetObjectArgs()
              .WithBucket(_bucketName)
            .WithObject(fileNameDB)
            .WithCallbackStream((stream) =>
            {
                stream.CopyTo(Console.OpenStandardOutput());
            }); ;

            await _minioClient.GetObjectAsync(args).ConfigureAwait(false);
            Console.WriteLine($"Downloaded the file {fileNameDB} from bucket {_bucketName}");
        }
    }


}

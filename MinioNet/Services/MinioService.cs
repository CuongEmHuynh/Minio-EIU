using Minio.DataModel.Args;
using Minio.Exceptions;
using Minio;
using MinioNet.Config;
using Microsoft.AspNetCore.Routing.Constraints;
using Minio.DataModel.Notification;

namespace MinioNet.Services
{

    public class MinioService
    {
        private readonly IMinioClient _minioClient;
        private readonly ILogger<MinioService> _logger;

        public MinioService(MinioConfiguration config, ILogger<MinioService> logger)
        {
            _minioClient = new MinioClient()
                                .WithEndpoint(config.Endpoint)
                                .WithCredentials(config.AccessKey, config.SecretKey)
                                .WithSSL(false)
                                .Build();
            _logger = logger;
        }

        /// <summary>
        /// Init Bucket in server minio
        /// </summary>
        /// <param name="bucketName"></param>
        /// <returns></returns>
        public async Task InitializeBucketAsync(string bucketName)
        {
            try
            {
                var bucketExistsArgs = new BucketExistsArgs().WithBucket(bucketName);
                bool found = await _minioClient.BucketExistsAsync(bucketExistsArgs).ConfigureAwait(false);

                if (!found)
                {
                    var makeBucketArgs = new MakeBucketArgs().WithBucket(bucketName);
                    await _minioClient.MakeBucketAsync(makeBucketArgs).ConfigureAwait(false);
                    _logger.LogInformation($"Bucket '{bucketName}' created successfully.");
                }
                else
                {
                    _logger.LogInformation($"Bucket '{bucketName}' already exists.");
                }
            }
            catch (MinioException ex)
            {
                _logger.LogError(ex, $"Error creating bucket '{bucketName}': {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Upload file at server minio
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="objectName"></param>
        /// <param name="filePath"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public async Task UploadFileAsync(string bucketName, IFormFile file, string pathFile)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is empty or null.");
            }

            //string objectName = string.IsNullOrEmpty(folder) ? file.FileName : $"{folder}/{file.FileName}";
            var fileUpload = file.OpenReadStream();
            var fileContentTpe = file.ContentType;

            try
            {
                // Make a bucket on the server, if not already present.
                var beArgs = new BucketExistsArgs()
                    .WithBucket(bucketName);
                bool found = await _minioClient.BucketExistsAsync(beArgs).ConfigureAwait(false);
                if (!found)
                {
                    var mbArgs = new MakeBucketArgs()
                        .WithBucket(bucketName);
                    await _minioClient.MakeBucketAsync(mbArgs).ConfigureAwait(false);
                }
                // Upload a file to bucket.
                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(pathFile)
                    .WithContentType(fileContentTpe ?? "application/octet-stream")
                    .WithStreamData(fileUpload).WithObjectSize(fileUpload.Length);

                await _minioClient.PutObjectAsync(putObjectArgs).ConfigureAwait(false);

                Console.WriteLine("Successfully uploaded " + pathFile);
            }
            catch (MinioException e)
            {
                Console.WriteLine("File Upload Error: {0}", e.Message);
            }
        }

        /// <summary>
        /// Download file from sever minio
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public async Task<MemoryStream> DownloadFileAsync(string bucketName, string pathFile)
        {
            var memoryStream = new MemoryStream();
            //objectName = string.IsNullOrEmpty(folder) ? objectName : $"{folder}/{objectName}";
            try
            {
                var getObjectArgs = new GetObjectArgs()
                                        .WithBucket(bucketName)
                                        .WithObject(pathFile)
                                        .WithCallbackStream(stream => stream.CopyTo(memoryStream));

                await _minioClient.GetObjectAsync(getObjectArgs);
                memoryStream.Position = 0;
                _logger.LogInformation($"Successfully downloaded '{pathFile}' from bucket '{bucketName}'.");
            }
            catch (MinioException ex)
            {
                _logger.LogError(ex, $"Error downloading file '{pathFile}' from bucket '{bucketName}': {ex.Message}");
                throw;
            }

            return memoryStream;
        }

        /// <summary>
        /// Remove file 
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public async Task DeleteFileAsync(string bucketName, string pathFile)
        {
            try
            {
                //objectName = string.IsNullOrEmpty(folder) ? objectName : $"{folder}/{objectName}";
                var removeObjectArgs = new RemoveObjectArgs()
                                          .WithBucket(bucketName)
                                          .WithObject(pathFile);

                await _minioClient.RemoveObjectAsync(removeObjectArgs).ConfigureAwait(false);
                _logger.LogInformation($"Successfully deleted '{pathFile}' from bucket '{bucketName}'.");
            }
            catch (MinioException ex)
            {
                _logger.LogError(ex, $"Error deleting file '{pathFile}' from bucket '{bucketName}': {ex.Message}");
                throw;
            }
        }
    }
}

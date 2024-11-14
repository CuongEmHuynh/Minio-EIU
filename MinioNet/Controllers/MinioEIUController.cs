using Microsoft.AspNetCore.Mvc;
using MinioNet.Dto;
using MinioNet.Services;

namespace MinioNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MinioEIUController : ControllerBase
    {
        private readonly MinioService _minioService;

        public MinioEIUController(MinioService minioService)
        {
            _minioService = minioService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] UploadFileParms parms)
        {
            var file = parms.File;
            string bucketName = parms.BucketName;
            if (string.IsNullOrWhiteSpace(bucketName))
            {
                return BadRequest("Bucket name cannot be empty.");
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest("File is empty or not provided.");
            }

            try
            {
                await _minioService.UploadFileAsync(bucketName, file, parms.PathFile);
                return Ok($"File '{file.FileName}' uploaded successfully to bucket '{bucketName}'.");
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, $"Error uploading file '{file.FileName}' to bucket '{bucketName}': {ex.Message}");
                return StatusCode(500, "An error occurred while uploading the file.");
            }
        }

        [HttpGet("download/{bucketName}")]
        public async Task<IActionResult> DownloadFile(string bucketName, string pathFile)
        {
            var memoryStream = await _minioService.DownloadFileAsync(bucketName, pathFile);
            return File(memoryStream, "application/octet-stream", pathFile);
        }

        [HttpDelete("delete/{bucketName}/{objectName}")]
        public async Task<IActionResult> DeleteFile(string bucketName, string pathFile)
        {
            await _minioService.DeleteFileAsync(bucketName,pathFile);
            return Ok($"File '{pathFile}' deleted successfully from bucket '{bucketName}'.");
        }
    }
}

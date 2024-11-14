namespace MinioNet.Dto
{
    public class UploadFileParms
    {
        public IFormFile File { get; set; }
        
        public string BucketName { get; set; }

        //public string? Folder {  get; set; }

        public string PathFile { get; set; }
    }
}

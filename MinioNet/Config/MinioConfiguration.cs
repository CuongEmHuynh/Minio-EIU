namespace MinioNet.Config
{
    public class MinioConfiguration
    {
        public string Endpoint { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public bool UseSSL { get; set; } = false;
    }
}

namespace ToyStoreManagement.Application.DTOs.Common
{
    public class MinioSettings
    {
        public string Endpoint { get; set; } = string.Empty;
        public string AccessKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public string BucketName { get; set; } = "toystore";
        public bool Secure { get; set; } = false;
    }
}

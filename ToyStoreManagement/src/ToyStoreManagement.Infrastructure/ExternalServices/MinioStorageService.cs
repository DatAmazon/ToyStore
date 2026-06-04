using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using System.Text.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using ToyStoreManagement.Application.DTOs.Common;
using ToyStoreManagement.Application.Interfaces;

namespace ToyStoreManagement.Infrastructure.ExternalServices
{
    public class MinioStorageService : IStorageService
    {
        private readonly IMinioClient _minioClient;
        private readonly MinioSettings _settings;

        public MinioStorageService(IOptions<MinioSettings> settings)
        {
            _settings = settings.Value;
            _minioClient = new MinioClient()
                .WithEndpoint(_settings.Endpoint)
                .WithCredentials(_settings.AccessKey, _settings.SecretKey)
                .WithSSL(_settings.Secure)
                .Build();
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string folder = "")
        {
            try
            {
                // 1. Đảm bảo bucket tồn tại
                var beArgs = new BucketExistsArgs().WithBucket(_settings.BucketName);
                bool found = await _minioClient.BucketExistsAsync(beArgs).ConfigureAwait(false);
                if (!found)
                {
                    var mbArgs = new MakeBucketArgs().WithBucket(_settings.BucketName);
                    await _minioClient.MakeBucketAsync(mbArgs).ConfigureAwait(false);
                }

                // 2. LUÔN CẬP NHẬT POLICY THÀNH PUBLIC READ (Đảm bảo FE xem được ảnh)
                var policy = new
                {
                    Version = "2012-10-17",
                    Statement = new[]
                    {
                        new
                        {
                            Action = new[] { "s3:GetObject" },
                            Effect = "Allow",
                            Principal = "*", // Cho phép mọi người truy cập đọc
                            Resource = new[] { $"arn:aws:s3:::{_settings.BucketName}/*" }
                        }
                    }
                };

                var setPolicyArgs = new SetPolicyArgs()
                    .WithBucket(_settings.BucketName)
                    .WithPolicy(JsonSerializer.Serialize(policy));
                await _minioClient.SetPolicyAsync(setPolicyArgs).ConfigureAwait(false);

                // 3. Tiến hành upload
                string objectName = string.IsNullOrEmpty(folder) ? fileName : $"{folder.TrimEnd('/')}/{fileName}";

                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(_settings.BucketName)
                    .WithObject(objectName)
                    .WithStreamData(fileStream)
                    .WithObjectSize(fileStream.Length)
                    .WithContentType(contentType);

                await _minioClient.PutObjectAsync(putObjectArgs).ConfigureAwait(false);

                return objectName;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error uploading file to MinIO: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteFileAsync(string fileKey, string folder = "")
        {
            try
            {
                var removeObjectArgs = new RemoveObjectArgs()
                    .WithBucket(_settings.BucketName)
                    .WithObject(fileKey);

                await _minioClient.RemoveObjectAsync(removeObjectArgs).ConfigureAwait(false);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string GetFileUrl(string fileKey, string folder = "")
        {
            string baseUrl = $"{(_settings.Secure ? "https" : "http")}://{_settings.Endpoint}/{_settings.BucketName}/{fileKey}";   
            return baseUrl;
        }
    }
}

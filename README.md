
# 🚀 Minio-EIU

**Minio-EIU** là một thư viện .NET đơn giản, dễ cấu hình giúp các dự án tích hợp nhanh với [MinIO](https://min.io/) – một giải pháp lưu trữ object giống S3. Dự án này cung cấp sẵn một API để upload, download, và quản lý file, đồng thời hỗ trợ khởi động MinIO bằng Docker cho môi trường phát triển local.

---

## 📦 Mục đích

- Cung cấp API .NET để giao tiếp với MinIO.
- Hỗ trợ cấu hình linh hoạt qua file `launchSettings.json` hoặc biến môi trường.
- Dễ dàng tích hợp lại vào các dự án khác trong môi trường nội bộ hoặc production.

---

## 🐳 Khởi động MinIO bằng Docker

Bạn có thể khởi động một MinIO server local với lệnh sau:

```bash
docker run -p 9000:9000 -p 9001:9001 \
  -e "MINIO_ROOT_USER=admin" \
  -e "MINIO_ROOT_PASSWORD=admin123" \
  quay.io/minio/minio server /data --console-address ":9001"
```

### 🔧 Cấu hình mặc định

| Thành phần         | Giá trị mặc định       |
|--------------------|------------------------|
| MinIO API Port     | `9000`                 |
| MinIO Console Port | `9001`                 |
| Access Key         | `admin`                |
| Secret Key         | `admin123`             |
| Bucket mẫu         | `eiu-files`            |

> ⚠️ Lưu ý: Không dùng mật khẩu mặc định trên môi trường production.

---

## ⚙️ Sử dụng thư viện Minio-EIU

Sau khi tích hợp thư viện vào dự án .NET của bạn, có thể sử dụng như sau:

```csharp
var minioService = new MinioStorageService(configuration);

await minioService.UploadFileAsync("eiu-files", "myfile.txt", fileStream);

var downloaded = await minioService.DownloadFileAsync("eiu-files", "myfile.txt");
```

---

## 🛠️ Cấu hình `MinioNet/Properties/launchSettings.json`

Ví dụ cấu hình:

```json
"profiles": {
  "MinioNet": {
    "commandName": "Project",
    "environmentVariables": {
      "MINIO_ENDPOINT": "http://localhost:9000",
      "MINIO_ACCESS_KEY": "admin",
      "MINIO_SECRET_KEY": "admin123",
      "MINIO_BUCKET": "eiu-files"
    }
  }
}
```

### 🧾 Giải thích các biến:

- `MINIO_ENDPOINT`: Địa chỉ MinIO server, ví dụ: `http://localhost:9000`
- `MINIO_ACCESS_KEY`: Tài khoản truy cập
- `MINIO_SECRET_KEY`: Mật khẩu truy cập
- `MINIO_BUCKET`: Tên bucket để sử dụng mặc định

### 🔄 Thay đổi cấu hình:

Để dùng với MinIO khác (ví dụ trên production), chỉ cần sửa các biến trên theo địa chỉ server thật.

---

## 📂 Gợi ý sử dụng file `.env`

Tạo file `.env` để dễ quản lý biến môi trường:

```env
MINIO_ENDPOINT=http://localhost:9000
MINIO_ACCESS_KEY=admin
MINIO_SECRET_KEY=admin123
MINIO_BUCKET=eiu-files
```

Sử dụng thư viện như `DotNetEnv` để load file `.env` trong code nếu muốn:

```csharp
DotNetEnv.Env.Load();
```

---

## 🔐 Lưu ý bảo mật

- Luôn sử dụng HTTPS trên production.
- Không để lộ access key và secret key trong source code.
- Sử dụng IAM policies nếu triển khai với MinIO phân quyền nâng cao.

---

## 📚 Tài liệu tham khảo

- [MinIO Documentation](https://min.io/docs/minio/linux/index.html)
- [MinIO .NET SDK](https://github.com/minio/minio-dotnet)

---

## 💡 Ví dụ nhanh

```bash
# Khởi động MinIO
docker run -p 9000:9000 -p 9001:9001 \
  -e "MINIO_ROOT_USER=admin" \
  -e "MINIO_ROOT_PASSWORD=admin123" \
  quay.io/minio/minio server /data --console-address ":9001"
```

```csharp
// Upload file
await minioService.UploadFileAsync("eiu-files", "test.txt", fileStream);

// Download file
var stream = await minioService.DownloadFileAsync("eiu-files", "test.txt");
```

---

## 📩 Đóng góp

Nếu bạn thấy hữu ích và muốn đóng góp thêm, hãy tạo pull request hoặc issue nhé!

---

Chúc bạn tích hợp MinIO vui vẻ! 🚀

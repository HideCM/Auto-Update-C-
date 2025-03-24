# Auto Update Tool

Công cụ tự động cập nhật cho ứng dụng Windows, được viết bằng C#.

## Cách sử dụng

1. Tạo file `version.txt` trong cùng thư mục với chương trình, chứa phiên bản hiện tại (ví dụ: `1.0.1`)

2. Tạo file `ignore.txt` (tùy chọn) để liệt kê các file hoặc thư mục không muốn cập nhật:
```
config.json
userdata/
```

3. Cấu hình URL trong file `Program.cs`:
   - `remoteVersionUrl`: URL để kiểm tra phiên bản mới
   - `updateUrl`: URL để tải file update.zip

4. Chạy chương trình:
```
dotnet run
```

## Cấu trúc thư mục mẫu

```
YourApp/
├── Program.exe
├── version.txt
├── ignore.txt
└── [các file khác của ứng dụng]
```

## Lưu ý

- File `version.txt` phải chứa một số phiên bản hợp lệ (ví dụ: `1.0.1`)
- File `update.zip` phải chứa các file cần cập nhật với cấu trúc thư mục giống như thư mục đích
- Các file trong `ignore.txt` sẽ được giữ nguyên khi cập nhật 
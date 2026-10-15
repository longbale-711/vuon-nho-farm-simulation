# Refactor dự án

## Tổ chức

- Domain là C# thuần, không dùng Unity API; Runtime, Editor và Test có assembly riêng.
- Module nghiệp vụ: Currency, Stats, Crops, Management, Economy, Market, Workers, Movement, Navigation, Persistence, Presentation.
- Bootstrap kết nối module; World tạo hình ảnh; Verification kiểm thử player.
- `FarmEconomy` phối hợp wallet/crop/management; collection cây chỉ đọc từ bên ngoài.
- `FarmGame` kết nối dịch vụ, không tự dựng thế giới, quản lý hàng chờ, tính đường chọn cây hoặc triển khai lưu dữ liệu.
- `FarmHud` hiển thị trạng thái; popup cây và quản lý độc lập. UI không tính lại công thức giá.
- Mỗi type có file riêng; test được chia theo nghiệp vụ.
- Build tools tách tạo scene, font, cấu hình player và thao tác build.

## Quy ước

`.editorconfig` định nghĩa naming và format theo Microsoft C# conventions, tương thích C# của Unity 2022.3: public/type PascalCase, parameter/local camelCase, private instance field `_camelCase`, private static field `s_camelCase`, Allman braces, access modifier tường minh và một khai báo mỗi dòng.

Đã loại bỏ XML summary và comment mô tả thao tác thông thường. Chỉ còn 5 comment giải thích làm tròn tỷ lệ, giá hàng đã thu hoạch, đường qua các cạnh grid, migration key JSON và import package bất đồng bộ.

Context interface giới hạn phụ thuộc của từng module; state handler riêng giữ hành vi nhân vật dễ đọc và chỉnh sửa.

## Tương thích

Giữ `.meta` khi di chuyển script/font. Scene được tạo bằng Unity Editor API. Save cũ với key camelCase được chuyển sang key PascalCase trước khi đọc; số dư lớn, cấp cây và nâng cấp được giữ nguyên. Save lỗi dùng backup, và dirty flag chỉ được xóa khi ghi save thành công.

## Kiểm chứng

- 24/24 EditMode tests đạt, gồm migration save cũ, backup, giá xem trước, giao dịch lỗi, giới hạn collection và tiền tệ âm.
- Biên dịch Unity không có warning C#.
- Kiểm tra whitespace với `dotnet format --verify-no-changes` đạt.
- Các script/assets di chuyển đều có metadata.
- Player smoke test đạt: nút xây/nâng cấp, 6 cây, 11 khách, 3 dân, 15 đơn liên tiếp và lưu ra đĩa.

Báo cáo: `TestResults/refactor-editmode.xml` và `Builds/Verification/result.txt`. APK được build lại tại `Builds/Android/VuonNho.apk`. Chưa kiểm thử trên thiết bị Android thật.

APK release build thành công, chữ ký v1/v2 hợp lệ và ZIP không có lỗi. SHA-256: `0259ae1712aab130b0d382c08cdae7070b3a519e9d495efe81e578841f829cdc`.

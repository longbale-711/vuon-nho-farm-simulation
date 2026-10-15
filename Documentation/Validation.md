# Báo cáo kiểm thử

## Đã đạt

- **APK Android release build thành công**, khoảng 29 MB, IL2CPP, ARM64 + ARMv7, package `com.orchard.vuonnho`, Android API tối thiểu 23/target 34, màn hình dọc. Kiểm tra ZIP không có lỗi và `apksigner verify` xác nhận chữ ký v1/v2 hợp lệ.
- Unity 2022.3.62f3 LTS import assets, biên dịch và tạo scene thành công.
- **17/17 EditMode tests**: tiền tệ vượt phạm vi double vẫn giữ 1 cent, format số, thay/gỡ buff theo nguồn, giao dịch không chi vượt số dư, cấp cây +10% giá gốc, buff riêng/toàn cục nhân nhau, giá lúc hái không đổi sau nâng cấp, thêm 2 khách, save khôi phục toàn bộ nâng cấp, từ chối save sai, giới hạn số khách.
- A* so sánh với Dijkstra trên **25 bản đồ ngẫu nhiên cố định seed**, cấm cắt góc và kiểm tra đường không tồn tại.
- Đường từ tất cả 6 cây tới 3 quầy và đường tới 12 vị trí hàng chờ đều đi được.
- Build bản macOS Development và chạy kịch bản tự động hai lần thành công; lần cuối sau chỉnh sửa UI.
- Kịch bản thực tế: trạng thái đầu 150 $, 1 khách; gọi nút Xây, chờ dân tự thu hoạch/giao/bán; gọi nút Nâng cấp; mở 6 cây; mua tất cả loại nâng cấp; tạo đủ 11 khách và 3 dân; hoàn thành **15 đơn tiếp theo không kẹt hàng chờ**; lưu ra đĩa và đọc lại đúng số dư/số khách.
- Ảnh render 1080×1920 kiểm tra HUD, popup xây/nâng cấp, nông trại đầy đủ và quản lý; đã sửa font, sprite, hướng dẫn và bóng.

## Báo cáo và ảnh

- [Tests/editmode.xml](Tests/editmode.xml): kết quả NUnit sau refactor, 24/24 đạt.
- [Tests/player-smoke-test.txt](Tests/player-smoke-test.txt): kết quả tích hợp trên player macOS.
- [Images](Images): sáu ảnh render game từ kịch bản kiểm thử.
- `Builds/Verification/save-test/`: save riêng của kiểm thử, không dùng cho người chơi.
- `Builds/Android/VuonNho.apk`: bản APK mới của project; SHA-256 `0259ae1712aab130b0d382c08cdae7070b3a519e9d495efe81e578841f829cdc`.

## Phạm vi

Gameplay đã chạy trên player macOS. Chưa kiểm thử cài đặt/cảm ứng trên thiết bị Android thật và chưa chạy trực tiếp APK demo để đối chiếu bố cục từng pixel. Bản game triển khai các hành vi trong Requirement.txt và dùng các tài nguyên đã cung cấp.


## Sau refactor

24/24 kiểm thử đạt; player smoke test đạt với 6 cây, 11 khách, 3 dân, 15 đơn tiếp theo và save roundtrip. Kiểm tra format đạt, không có warning C# và không thiếu metadata. Xem [Refactor.md](Refactor.md) và [Tests/editmode.xml](Tests/editmode.xml).

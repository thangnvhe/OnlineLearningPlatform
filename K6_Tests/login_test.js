import http from "k6/http";
import { check, sleep } from "k6";

// 1. Cấu hình bài test (Chiến thuật tấn công)
export const options = {
  // Kịch bản: Bơm từ từ lên 100 user, giữ nguyên 30s, rồi giảm dần
  stages: [
    { duration: "10s", target: 50 }, // Trong 10s đầu: Tăng dần lên 50 user đồng thời
    { duration: "30s", target: 100 }, // 30s tiếp theo: Giữ mức 100 user liên tục spam
    { duration: "10s", target: 0 }, // 10s cuối: Hạ nhiệt dần về 0
  ],
  // Tùy chọn bỏ qua lỗi SSL khi test ở localhost (HTTPS tự sinh của .NET)
  insecureSkipTLSVerify: true,
};

// 2. Hành động của từng User giả lập
export default function () {
  // LƯU Ý: Đổi số port 5000 thành port mà API .NET của bạn đang chạy
  const url = "https://localhost:5000/api/Auth/Login";

  const payload = JSON.stringify({
    username: "baovnhe123", // Nhập 1 tài khoản có thật dưới DB của bạn
    password: "a123456",
  });

  const params = {
    headers: {
      "Content-Type": "application/json",
    },
  };

  // Bắn request POST lên API
  const res = http.post(url, payload, params);

  // Kiểm tra xem API có trả về code 200 (Thành công) không
  check(res, {
    "is status 200": (r) => r.status === 200,
    // Kiểm tra thêm xem JSON trả về có chữ "Success" không (dựa trên ServiceResult của bạn)
    "is success true": (r) =>
      r.body.includes('"isSuccess":true') ||
      r.body.includes('"succeeded":true'),
  });

  // Mô phỏng người dùng thực tế: Đăng nhập xong thì dừng 1 giây rồi mới thao tác tiếp
  sleep(1);
}

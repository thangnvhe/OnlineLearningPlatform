import http from "k6/http";
import { check, sleep } from "k6";

export const options = {
  stages: [
    { duration: "10s", target: 50 }, // Tăng dần lên 50 user
    { duration: "20s", target: 100 }, // Giữ 100 user test độ lỳ của Redis
    { duration: "10s", target: 0 }, // Hạ nhiệt
  ],
  insecureSkipTLSVerify: true,
};

const BASE_URL = "https://localhost:5000/api/Auth";

export default function () {
  // --- BƯỚC 1: ĐĂNG NHẬP ĐỂ CÓ COOKIE ---
  const loginPayload = JSON.stringify({
    username: "baovnhe123",
    password: "a123456",
  });

  const params = {
    headers: { "Content-Type": "application/json" },
  };

  const loginRes = http.post(`${BASE_URL}/Login`, loginPayload, params);

  // Kiểm tra login thành công
  check(loginRes, {
    "login success": (r) => r.status === 200,
  });

  // K6 tự động lưu trữ Cookie từ response vào "Cookie Jar"
  // --- BƯỚC 2: GỌI REFRESH TOKEN ---
  // API RefreshTokenAsync của bạn đọc từ Cookie nên không cần truyền Payload
  const refreshRes = http.post(`${BASE_URL}/RefreshToken`, null, params);

  check(refreshRes, {
    "refresh status is 200": (r) => r.status === 200,
    "has new access token": (r) => r.json().result.accessToken !== undefined,
    "is success true": (r) => r.json().isSuccess === true,
  });

  sleep(1);
}

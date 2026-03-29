import http from "k6/http";
import { check, sleep } from "k6";

export const options = { vus: 1, iterations: 1, insecureSkipTLSVerify: true };

export default function () {
  const email = __ENV.TEST_EMAIL || "thangshk08@gmail.com";
  const res = http.post(
    `${__ENV.BASE_URL || "https://localhost:5000"}/api/Auth/ForgotPassword`,
    JSON.stringify({ Email: email }),
    { headers: { "Content-Type": "application/json" } },
  );

  check(res, {
    "forgot password handled": (r) =>
      r.status === 200 || r.status === 400 || r.status === 404,
  });
  console.log("ForgotPassword for", email, "status=", res.status);
  sleep(1);
}

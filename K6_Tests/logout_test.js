import http from "k6/http";
import { check, sleep } from "k6";

export const options = { vus: 1, iterations: 1, insecureSkipTLSVerify: true };

function json(res) {
  try {
    return res.json();
  } catch (e) {
    return null;
  }
}

export default function () {
  const username = __ENV.TEST_USERNAME || "baovnhe123";
  const password = __ENV.TEST_PASSWORD || "NewP@ssw0rd1";

  const res = http.post(
    `${__ENV.BASE_URL || "https://localhost:5000"}/api/Auth/Login`,
    JSON.stringify({ Username: username, Password: password }),
    { headers: { "Content-Type": "application/json" } },
  );
  check(res, { "login status 200": (r) => r.status === 200 });
  const body = json(res);
  const token =
    (body && (body.accessToken || (body.result && body.result.accessToken))) ||
    null;
  check(token, { "has token": (t) => !!t });

  if (token) {
    const out = http.post(
      `${__ENV.BASE_URL || "https://localhost:5000"}/api/Auth/Logout`,
      null,
      {
        headers: {
          Authorization: `Bearer ${token}`,
          "Content-Type": "application/json",
        },
      },
    );
    check(out, { "logout status 200": (r) => r.status === 200 });
    console.log("Logout status=", out.status);
  }
  sleep(1);
}

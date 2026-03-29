import http from "k6/http";
import { check, sleep } from "k6";
import encoding from "k6/encoding";

export const options = { vus: 1, iterations: 1, insecureSkipTLSVerify: true };

function json(res) {
  try {
    return res.json();
  } catch (e) {
    return null;
  }
}
function parseJwt(token) {
  if (!token) return null;
  try {
    const parts = token.split(".");
    if (parts.length < 2) return null;
    let payload = parts[1].replace(/-/g, "+").replace(/_/g, "/");
    const pad = payload.length % 4;
    if (pad) payload += "=".repeat(4 - pad);
    const dec = encoding.b64decode(payload);
    let s = dec;
    if (typeof dec !== "string") {
      const u8 = new Uint8Array(dec);
      s = Array.from(u8)
        .map((c) => String.fromCharCode(c))
        .join("");
    }
    return JSON.parse(s);
  } catch (e) {
    return null;
  }
}

export default function () {
  const username = __ENV.TEST_USERNAME || "baovnhe123";
  const password = __ENV.TEST_PASSWORD || "a123456";
  const newPassword = __ENV.TEST_NEW_PASSWORD || "NewP@ssw0rd1";

  // login
  let res = http.post(
    `${__ENV.BASE_URL || "https://localhost:5000"}/api/Auth/Login`,
    JSON.stringify({ Username: username, Password: password }),
    { headers: { "Content-Type": "application/json" } },
  );
  check(res, { "login status 200": (r) => r.status === 200 });

  const body = json(res);
  const token =
    (body && (body.accessToken || (body.result && body.result.accessToken))) ||
    (body && body.data && body.data.accessToken) ||
    null;
  check(token, { "token present": (t) => !!t });

  const claims = parseJwt(token);
  // Đưa nameidentifier lên đầu tiên và tuyệt đối KHÔNG dùng claims.sub ở đây
  const userId =
    claims &&
    (claims[
      "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
    ] ||
      claims.nameid ||
      claims.Id);
  check(userId, { "userId extracted": (id) => !!id });

  if (userId) {
    res = http.post(
      `${__ENV.BASE_URL || "https://localhost:5000"}/api/Auth/ChangePassword`,
      JSON.stringify({
        UserId: userId,
        CurrentPassword: password,
        NewPassword: newPassword,
      }),
      {
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
      },
    );
    check(res, { "change password status 200": (r) => r.status === 200 });
    console.log("ChangePassword status=", res.status, "body=", res.body);
  }

  sleep(1);
}

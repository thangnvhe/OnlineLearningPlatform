import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = { vus: 1, iterations: 1, insecureSkipTLSVerify: true };

function uniqueEmail() { return `k6_${Date.now()}@example.com`; }

export default function () {
  const username = `k6user_${Math.floor(Math.random()*10000)}`;
  const email = __ENV.TEST_EMAIL || uniqueEmail();
  const password = __ENV.TEST_PASSWORD || 'Test@1234';

  const payload = JSON.stringify({ Username: username, Email: email, Password: password, FirstName: 'K6', LastName: 'User' });
  const res = http.post(`${__ENV.BASE_URL || 'https://localhost:5000'}/api/Auth/Register`, payload, { headers: { 'Content-Type':'application/json' } });

  check(res, {
    'register status 200 or 201': r => r.status === 200 || r.status === 201,
  });

  // Print useful data for manual verification
  console.log('Registered:', username, email, 'status=', res.status);
  sleep(1);
}

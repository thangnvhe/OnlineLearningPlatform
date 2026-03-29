import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = { vus: 1, iterations: 1, insecureSkipTLSVerify: true };

export default function(){
  const username = __ENV.TEST_USERNAME || 'baovnhe123';
  const res = http.post(`${__ENV.BASE_URL || 'https://localhost:5000'}/api/Auth/ResendConfirmationEmail`, JSON.stringify({ Username: username }), { headers:{ 'Content-Type':'application/json' } });
  check(res, { 'resend confirmation handled': r => r.status === 200 || r.status === 400 || r.status === 404 });
  console.log('ResendConfirmation for', username, 'status=', res.status);
  sleep(1);
}

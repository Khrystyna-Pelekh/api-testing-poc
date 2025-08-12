import http from 'k6/http';
import { check, group, sleep } from 'k6';
import { uuidv4 } from 'https://jslib.k6.io/k6-utils/1.4.0/index.js';

// Основні опції навантажувального тесту.
// Hалаштовуємо кількість віртуальних користувачів (VUs) та тривалість тесту.
export const options = {
  scenarios: {
    loadTestScenario: {
      executor: 'constant-vus', 
      vus: 20,                
      duration: '30s',         
    },
  },

  // Порогові значення, при перевищенні яких тест буде вважатися невдалим.
  thresholds: {
    'http_req_failed': ['rate<0.01'], 
    'http_req_duration{name:GET}': ['p(90)<300', 'p(95)<400'],
    'http_req_duration{name:POST}': ['p(90)<400', 'p(95)<500'],
    'http_req_duration{name:PUT}': ['p(90)<400', 'p(95)<500'],
    'http_req_duration{name:DELETE}': ['p(90)<400', 'p(95)<500'],
  },
};

export default function () {
  const BASE_URL = 'https://fakerestapi.azurewebsites.net/api/v1';
  const staticId = 1;

  group('API Performance Test - CRUD Flow', function () {
    // 1. Створюємо новий ресурс (POST-запит)
    const uniqueTitle = `Activity ${uuidv4()}`;
    const payload = JSON.stringify({
      title: uniqueTitle,
      completed: false,
    });
    const headers = {
      'Content-Type': 'application/json',
    };

    let postResponse = http.post(`${BASE_URL}/Activities`, payload, {
      headers,
      tags: { name: 'POST' },
    });

    check(postResponse, {
      'POST status is 200': (r) => r.status === 200,
      'POST response has a title': (r) => r.json().title === uniqueTitle,
    });
    sleep(1);

    // 2. Отримуємо існуючий ресурс (GET-запит)
    let getResponse = http.get(`${BASE_URL}/Activities/${staticId}`, {
      tags: { name: 'GET' },
    });
    
    check(getResponse, {
      'GET status is 200': (r) => r.status === 200,
      'GET response has correct ID': (r) => r.json().id === staticId,
    });
    sleep(1);

    // 3. Оновлюємо існуючий ресурс (PUT-запит)
    const updatedTitle = `Updated Activity ${uuidv4()}`;
    const updatePayload = JSON.stringify({
      id: staticId,
      title: updatedTitle,
      completed: true,
    });

    let putResponse = http.put(`${BASE_URL}/Activities/${staticId}`, updatePayload, {
      headers,
      tags: { name: 'PUT' },
    });

    check(putResponse, {
      'PUT status is 200': (r) => r.status === 200,
      'PUT response has updated title': (r) => r.json().title === updatedTitle,
    });
    sleep(1);

    // 4. Видаляємо існуючий ресурс (DELETE-запит)
    let deleteResponse = http.del(`${BASE_URL}/Activities/${staticId}`, null, {
      tags: { name: 'DELETE' },
    });

    check(deleteResponse, {
      'DELETE status is 200': (r) => r.status === 200,
    });
    sleep(1);
  });
}

import axios from 'axios';

const api = axios.create({
  baseURL: process.env.REACT_APP_API_URL || 'http://localhost:5055/api',
  timeout: 15000,
});

const isAuthEndpoint = (url) =>
  url?.includes('/Auth/login') || url?.includes('/Auth/register');

export const getApiErrorMessage = (error, fallback = 'Something went wrong') => {
  const data = error.response?.data;

  if (typeof data === 'string') {
    try {
      const parsed = JSON.parse(data);
      if (typeof parsed?.message === 'string') return parsed.message;
    } catch {
      return data || fallback;
    }
  }

  if (typeof data?.message === 'string') return data.message;

  if (typeof data?.title === 'string') return data.title;

  if (data?.errors) {
    const messages = Object.values(data.errors).flat();
    if (messages.length) return messages.join(' ');
  }

  if (error.response?.status === 401) return 'Invalid email or password.';

  return error.message || fallback;
};

api.interceptors.request.use((config) => {
  if (isAuthEndpoint(config.url)) return config;

  const token = localStorage.getItem('authToken');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.code === 'ECONNABORTED') {
      error.message = 'Server not responding. Make sure backend is running on port 5055.';
    } else if (!error.response) {
      error.message = 'Cannot reach server. Run: docker compose up -d --build';
    } else if (error.response?.status === 401) {
      const onLoginPage = isAuthEndpoint(error.config?.url);

      if (!onLoginPage) {
        localStorage.removeItem('authToken');
        localStorage.removeItem('authUser');
        if (window.location.pathname !== '/login') {
          window.location.href = '/login';
        }
      }

      error.message = getApiErrorMessage(error, 'Invalid email or password.');
    } else {
      error.message = getApiErrorMessage(error, error.message);
    }

    return Promise.reject(error);
  }
);

export default api;

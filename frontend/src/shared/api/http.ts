import axios from 'axios';

const usersBaseUrl = import.meta.env.VITE_USERS_API;
const productsBaseUrl = import.meta.env.VITE_PRODUCTS_API;

export const usersApi = axios.create({
  baseURL: `${usersBaseUrl}/api`,
});

export const productsApi = axios.create({
  baseURL: `${productsBaseUrl}/api`,
});

[usersApi, productsApi].forEach((instance) => {
  instance.interceptors.request.use((config) => {
    const token = localStorage.getItem('accessToken');
    if (token) {
      config.headers = config.headers ?? {};
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  });
});

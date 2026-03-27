import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:5000/api',
});

// Attach JWT automatically
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

// Auth
export const loginClient  = (data) => api.post('/auth/login-client',  data);
export const loginAgent   = (data) => api.post('/auth/login-agent',   data);
export const registerClient = (data) => api.post('/auth/register-client', data);

// Referentiels
export const getReferentiels = () => api.get('/referentiels');

// Tickets
export const getTickets     = () => api.get('/tickets');
export const getTicket      = (id) => api.get(`/tickets/${id}`);
export const createTicket   = (data) => api.post('/tickets', data);
export const updateStatus   = (id, data) => api.put(`/tickets/${id}/status`, data);
export const getHistory     = (id) => api.get(`/tickets/${id}/history`);

// Messages
export const getMessages    = (id) => api.get(`/tickets/${id}/messages`);
export const sendMessage    = (id, formData) =>
  api.post(`/tickets/${id}/messages`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  });

// Agent
export const getAgentDashboard = () => api.get('/agent/dashboard');
export const getAgentTickets   = (params) => api.get('/agent/tickets', { params });

export default api;

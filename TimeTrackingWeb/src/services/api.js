import axios from 'axios';

const api = axios.create({
  baseURL: 'https://localhost:7209/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

export const projectsApi = {
  getAll: () => api.get('/Projects'),
  getById: (id) => api.get(`/Projects/${id}`),
  create: (data) => api.post('/Projects', data),
  update: (id, data) => api.put(`/Projects/${id}`, data),
  delete: (id) => api.delete(`/Projects/${id}`),
};

export const tasksApi = {
  getAll: () => api.get('/Tasks'),
  getActive: () => api.get('/Tasks/active'),
  getByProject: (projectId) => api.get(`/Tasks/by-project/${projectId}`),
  getById: (id) => api.get(`/Tasks/${id}`),
  create: (data) => api.post('/Tasks', data),
  update: (id, data) => api.put(`/Tasks/${id}`, data),
  delete: (id) => api.delete(`/Tasks/${id}`),
};

export const timeEntriesApi = {
  getAll: () => api.get('/TimeEntries'),
  getByDate: (date) => api.get(`/TimeEntries/by-date/${date}`),
  getByMonth: (year, month) => api.get(`/TimeEntries/by-month/${year}/${month}`),
  getDailySummary: (employeeId, date) => 
    api.get(`/TimeEntries/daily-summary/${employeeId}/${date}`),
  getById: (id) => api.get(`/TimeEntries/${id}`),
  create: (data) => api.post('/TimeEntries', data),
  update: (id, data) => api.put(`/TimeEntries/${id}`, data),
  delete: (id) => api.delete(`/TimeEntries/${id}`),
};

export { api };

export default api;
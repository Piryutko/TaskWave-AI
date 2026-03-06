// API base URL — can override via env variable
export const API_BASE = typeof import.meta !== 'undefined' && import.meta.env?.VITE_API_URL
    ? import.meta.env.VITE_API_URL
    : 'http://localhost:5000';

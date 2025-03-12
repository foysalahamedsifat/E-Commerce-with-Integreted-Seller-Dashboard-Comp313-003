import axios from "axios";

const API_URL = "https://localhost:7048/api"; // Change if backend URL is different

const api = axios.create({
  baseURL: API_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

// Set Auth Token for Requests
export const setAuthToken = (token) => {
  if (token) {
    api.defaults.headers.common["Authorization"] = token ? `Bearer ${token}` : "";
  }
};

// Authentication APIs
export const login = (data) => api.post("/Authenticate/login", data);
export const register = (data) => api.post("/Authenticate/register", data);

// Product APIs
export const getProducts = () => api.get("/product");
export const getProductById = (id) => api.get(`/product/${id}`);

// Cart APIs
export const addToCart = (data) => api.post("/cart", data);
export const getCart = () => api.get("/cart");
export const removeFromCart = (id) => api.delete(`/cart/${id}`);

// Order APIs
export const placeOrder = (data) => api.post("/order", data);
export const getOrders = () => api.get("/order");
export const getUserOrders = () => api.get("/order/user");


// Admin Analytics API (Add this to fix the import error)
export const getAdminAnalytics = () => api.get("/admin/analytics");
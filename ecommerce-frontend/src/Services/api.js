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
    api.defaults.headers.common["Authorization"] = `Bearer ${token}`;
  }
};

// Authentication APIs
export const login = (data) => api.post("/Authenticate/login", data);
export const register = (data) => api.post("/Authenticate/register", data);
export const registerAdmin = (data) => api.post("/Authenticate/register-admin", data);

// Product APIs
export const getProducts = () => api.get("/product");
export const getProductById = (id) => api.get(`/product/${id}`);

// Admin Product Management APIs
export const addProduct = (productData) => 
  api.post("/product", productData, { headers: { "Content-Type": "multipart/form-data" } });

export const updateProduct = (id, productData) => 
  api.put(`/product/${id}`, productData, { headers: { "Content-Type": "multipart/form-data" } });

export const deleteProduct = (id) => api.delete(`/product/${id}`);

// Cart APIs
export const addToCart = (data) => api.post("/cart", data);
export const getCart = () => api.get("/cart");
export const removeFromCart = (id) => api.delete(`/cart/${id}`);

// Order APIs
export const placeOrder = (data) => api.post("/order", data);
export const getOrders = () => api.get("/order");
export const getUserOrders = () => api.get("/order/user");

// Admin APIs
export const getAdminAnalytics = () => api.get("/admin/analytics"); // Added this to fix the error

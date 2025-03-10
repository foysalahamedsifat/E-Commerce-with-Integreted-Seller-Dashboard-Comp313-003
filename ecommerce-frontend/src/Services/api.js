import axios from "axios";

const API_URL = "http://localhost:7048/api"; // Change if backend URL is different

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
  } else {
    delete api.defaults.headers.common["Authorization"];
  }
};

// Authentication APIs
export const login = (data) => api.post("/auth/login", data);
export const register = (data) => api.post("/auth/register", data);

// Product APIs
export const getProducts = () => api.get("/products");
export const getProductById = (id) => api.get(`/products/${id}`);

// Cart APIs
export const addToCart = (data) => api.post("/cart", data);
export const getCart = () => api.get("/cart");
export const removeFromCart = (id) => api.delete(`/cart/${id}`);

// Order APIs
export const placeOrder = (data) => api.post("/orders", data);
export const

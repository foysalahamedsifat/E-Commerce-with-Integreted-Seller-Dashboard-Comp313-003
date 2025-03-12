import React from "react";
import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import Home from "./pages/Home";
import Login from "./pages/Login";
import Checkout from "./pages/Checkout";
import Orders from "./pages/Orders";
import AdminDashboard from "./pages/AdminDashboard";
import Navbar from "./components/Navbar";
import Cart from "./pages/cart";
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css"; // Make sure you import the CSS!
import { useDispatch } from "react-redux";
import { loginSuccess } from "./redux/authSlice"; // adjust path
import { useEffect } from "react";
import Registration from "./pages/registration";
import RegistrationAdmin from "./pages/registration-admin";

const App = () => {
  const dispatch = useDispatch();

  useEffect(() => {
    const token = localStorage.getItem("token");

    if (token) {
      // You might want to fetch user details again, or store cached user info
      dispatch(loginSuccess({ user: null, token })); // or include user if you have it
    }
  }, [dispatch]);

  return (
    <Router>
      <Navbar />

      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/login" element={<Login />} />
        <Route path="/checkout" element={<Checkout />} />
        <Route path="/orders" element={<Orders />} />
        <Route path="/admin" element={<AdminDashboard />} />
        <Route path="/cart" element={<Cart />} />
        <Route path="/registration" element={<Registration />} />
        <Route path="/registration-admin" element={<RegistrationAdmin />} />


      </Routes>

      {/* ToastContainer should be outside of Routes */}
      <ToastContainer position="top-right" autoClose={3000} />
    </Router>
  );
};

export default App;

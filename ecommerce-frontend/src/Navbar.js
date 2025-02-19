import React from "react";
import { Link, useNavigate } from "react-router-dom";
import { Button } from "@mui/material";

const Navbar = () => {
  const navigate = useNavigate();
  
  const handleLogout = () => {
    localStorage.removeItem("token");
    navigate("/login");
  };

  return (
    <nav>
      <Link to="/">Home</Link>
      <Link to="/login">Login</Link>
      <Link to="/register">Register</Link>
      <Button onClick={handleLogout} color="secondary">Logout</Button>
    </nav>
  );
};

export default Navbar;

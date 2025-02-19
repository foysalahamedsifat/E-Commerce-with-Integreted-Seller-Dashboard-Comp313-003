import React, { useState } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";
import {jwtDecode} from "jwt-decode";
import { TextField, Button, Container, Typography, Paper } from "@mui/material";

const Login = () => {
  const [formData, setFormData] = useState({ username: "", password: "" });
  const navigate = useNavigate();

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleLogin = async (e) => {
    e.preventDefault();
    try {
      const response = await axios.post(`${process.env.REACT_APP_API_URL}/authenticate/login`, formData);
      localStorage.setItem("token", response.data.token);
      
      // Decode token to get user info (optional)
      const user = jwtDecode(response.data.token);
      console.log("User logged in:", user);

      navigate("/dashboard");
    } catch (error) {
      console.error("Login failed:", error);
      alert("Invalid username or password.");
    }
  };

  return (
    <Container component="main" maxWidth="xs">
      <Paper elevation={3} style={{ padding: "2rem", marginTop: "3rem" }}>
        <Typography variant="h5" gutterBottom>
          Login to Your Account
        </Typography>
        <form onSubmit={handleLogin}>
          <TextField fullWidth label="Username" name="username" onChange={handleChange} margin="normal" required />
          <TextField fullWidth label="Password" name="password" type="password" onChange={handleChange} margin="normal" required />
          <Button type="submit" fullWidth variant="contained" color="primary" style={{ marginTop: "1rem" }}>
            Login
          </Button>
        </form>
      </Paper>
    </Container>
  );
};

export default Login;

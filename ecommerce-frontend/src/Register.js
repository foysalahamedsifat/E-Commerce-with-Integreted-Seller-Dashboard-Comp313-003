import React, { useState } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";
import { TextField, Button, Container, Typography, Paper } from "@mui/material";

const Register = () => {
  const [formData, setFormData] = useState({ username: "", email: "", password: "" });
  const navigate = useNavigate();

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleRegister = async (e) => {
    e.preventDefault();
    try {
      await axios.post(`${process.env.REACT_APP_API_URL}/authenticate/register`, formData);
      alert("Registration successful! Please log in.");
      navigate("/login");
    } catch (error) {
      console.error("Registration failed:", error);
      alert("Error: Unable to register");
    }
  };

  return (
    <Container component="main" maxWidth="xs">
      <Paper elevation={3} style={{ padding: "2rem", marginTop: "3rem" }}>
        <Typography variant="h5" gutterBottom>
          Create an Account
        </Typography>
        <form onSubmit={handleRegister}>
          <TextField fullWidth label="Username" name="username" onChange={handleChange} margin="normal" required />
          <TextField fullWidth label="Email" name="email" type="email" onChange={handleChange} margin="normal" required />
          <TextField fullWidth label="Password" name="password" type="password" onChange={handleChange} margin="normal" required />
          <Button type="submit" fullWidth variant="contained" color="primary" style={{ marginTop: "1rem" }}>
            Register
          </Button>
        </form>
      </Paper>
    </Container>
  );
};

export default Register;

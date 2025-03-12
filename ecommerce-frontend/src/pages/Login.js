import React, { useState } from "react";
import { login } from "../Services/api";
import { useDispatch } from "react-redux";
import { loginSuccess } from "../redux/authSlice";
import { Form, Button, Container } from "react-bootstrap";
import { useNavigate } from "react-router-dom";

const Login = () => {
  const [form, setForm] = useState({ email: "", password: "" });
  const dispatch = useDispatch();
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    const res = await login(form);
    dispatch(loginSuccess(res.data));
    navigate("/");
  };

  return (
    <Container className="mt-4">
      <h2>Login</h2>
      <Form onSubmit={handleSubmit}>
        <Form.Group className="mb-3">
          <Form.Label>Email</Form.Label>
          <Form.Control type="text" onChange={(e) => setForm({ ...form, email: e.target.value })} />
        </Form.Group>
        <Form.Group className="mb-3">
          <Form.Label>Password</Form.Label>
          <Form.Control type="password" onChange={(e) => setForm({ ...form, password: e.target.value })} />
        </Form.Group>
        <Button type="submit">Login</Button>
      </Form>
    </Container>
  );
};

export default Login;

import React, { useState } from "react";
import { login } from "../Services/api";
import { useDispatch } from "react-redux";
import { loginSuccess } from "../redux/authSlice";
import { Form, Button, Container, Alert } from "react-bootstrap";
import { useNavigate } from "react-router-dom";

const Login = () => {
  const [form, setForm] = useState({ username: "", password: "" });
  const [errors, setErrors] = useState(null); // ✅ For API error responses
  const dispatch = useDispatch();
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setErrors(null); // Clear previous errors

    try {
      const res = await login(form);
      dispatch(loginSuccess(res.data));
      navigate("/");
    } catch (error) {
      if (error.response) {
        const apiError = error.response.data;

        // ✅ Handle validation errors (like the example you shared)
        if (apiError.errors) {
          const messages = [];

          for (const key in apiError.errors) {
            if (Object.hasOwn(apiError.errors, key)) {
              messages.push(...apiError.errors[key]);
            }
          }

          setErrors(messages);
        } else if (apiError.title) {
          // ✅ Handle other errors with a title
          setErrors([apiError.title]);
        } else {
          // ✅ Fallback for unknown errors
          setErrors(["An unexpected error occurred."]);
        }
      } else {
        setErrors(["Unable to connect to the server."]);
      }
    }
  };

  return (
    <Container className="mt-4" style={{ maxWidth: "400px" }}>
      <h2 className="mb-4">Login</h2>

      {/* ✅ Display Errors */}
      {errors &&
        errors.map((err, idx) => (
          <Alert key={idx} variant="danger">
            {err}
          </Alert>
        ))}

      <Form onSubmit={handleSubmit}>
        <Form.Group className="mb-3">
          <Form.Label>Username</Form.Label>
          <Form.Control
            type="text"
            value={form.username}
            onChange={(e) => setForm({ ...form, username: e.target.value })}
            required
          />
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>Password</Form.Label>
          <Form.Control
            type="password"
            value={form.password}
            onChange={(e) => setForm({ ...form, password: e.target.value })}
            required
          />
        </Form.Group>

        <Button type="submit" variant="primary" className="w-100">
          Login
        </Button>
      </Form>
    </Container>
  );
};

export default Login;

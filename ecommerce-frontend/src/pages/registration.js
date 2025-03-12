import React, { useState } from "react";
import { register } from "../Services/api"; // Adjust this import as needed
import { useNavigate } from "react-router-dom";
import { Form, Button, Container, Alert } from "react-bootstrap";

const Registration = () => {
  const navigate = useNavigate();

  const [formData, setFormData] = useState({
    username: "",
    email: "",
    password: "",
    firstName: "",
    lastName: "",
    address: "",
    phoneNumber: "",
    postalCode: ""
  });

  const [errors, setErrors] = useState(null);
  const [success, setSuccess] = useState(null);

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setErrors(null);
    setSuccess(null);

    try {
      const res = await register(formData);

      // ✅ Success message
      setSuccess(res.data.message || "Registration successful!");

      // ✅ Redirect after a short delay
      setTimeout(() => navigate("/login"), 2000);
    } catch (err) {
      if (err.response) {
        const apiError = err.response.data;

        if (apiError.errors) {
          const messages = [];

          for (const key in apiError.errors) {
            if (Object.hasOwn(apiError.errors, key)) {
              messages.push(...apiError.errors[key]);
            }
          }

          setErrors(messages);
        } else if (apiError.title) {
          setErrors([apiError.title]);
        } else {
          setErrors([apiError.message || "Registration failed"]);
        }
      } else {
        setErrors(["Unable to connect to the server."]);
      }
    }
  };

  return (
    <Container className="mt-4" style={{ maxWidth: "500px" }}>
      <h2 className="mb-4">User Registration</h2>

      {/* ✅ Success Message */}
      {success && (
        <Alert variant="success">
          {success}
        </Alert>
      )}

      {/* ✅ Error Messages */}
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
            name="username"
            value={formData.username}
            onChange={handleChange}
            placeholder="Enter username"
            required
          />
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>Email</Form.Label>
          <Form.Control
            type="email"
            name="email"
            value={formData.email}
            onChange={handleChange}
            placeholder="Enter email"
            required
          />
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>Password</Form.Label>
          <Form.Control
            type="password"
            name="password"
            value={formData.password}
            onChange={handleChange}
            placeholder="Enter password"
            required
          />
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>First Name</Form.Label>
          <Form.Control
            type="text"
            name="firstName"
            value={formData.firstName}
            onChange={handleChange}
            placeholder="Enter first name"
            required
          />
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>Last Name</Form.Label>
          <Form.Control
            type="text"
            name="lastName"
            value={formData.lastName}
            onChange={handleChange}
            placeholder="Enter last name"
            required
          />
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>Address</Form.Label>
          <Form.Control
            type="text"
            name="address"
            value={formData.address}
            onChange={handleChange}
            placeholder="Enter address"
            required
          />
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>Phone Number</Form.Label>
          <Form.Control
            type="text"
            name="phoneNumber"
            value={formData.phoneNumber}
            onChange={handleChange}
            placeholder="Enter phone number"
            required
          />
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>Postal Code</Form.Label>
          <Form.Control
            type="text"
            name="postalCode"
            value={formData.postalCode}
            onChange={handleChange}
            placeholder="Enter postal code"
            required
          />
        </Form.Group>

        <Button type="submit" variant="primary" className="w-100">
          Register
        </Button>
      </Form>
    </Container>
  );
};

export default Registration;

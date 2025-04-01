import React, { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { getProductById, addProduct, updateProduct } from "../Services/api";
import { Form, Button, Container, Alert } from "react-bootstrap";

const ProductForm = () => {
  const { id } = useParams(); // Get product ID from URL (if editing)
  const navigate = useNavigate();
  const [product, setProduct] = useState({
    name: "",
    description: "",
    price: "",
    stock: "",
    image: null,
  });
  const [message, setMessage] = useState("");
  const [error, setError] = useState("");

  useEffect(() => {
    if (id) {
      getProductById(id)
        .then((res) => setProduct(res.data))
        .catch(() => setError("Failed to load product details"));
    }
  }, [id]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setProduct((prev) => ({ ...prev, [name]: value }));
  };

  const handleFileChange = (e) => {
    setProduct((prev) => ({ ...prev, image: e.target.files[0] }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    setMessage("");
    
    try {
      const formData = new FormData();
      formData.append("name", product.name);
      formData.append("description", product.description);
      formData.append("price", product.price);
      formData.append("stock", product.stock);
      if (product.image) {
        formData.append("image", product.image);
      }

      if (id) {
        await updateProduct(id, formData);
        setMessage("Product updated successfully!");
      } else {
        await addProduct(formData);
        setMessage("Product added successfully!");
      }
      setTimeout(() => navigate("/admin/products"), 2000);
    } catch (err) {
      setError("Something went wrong. Please try again.");
    }
  };

  return (
    <Container className="mt-4">
      <h2>{id ? "Edit Product" : "Add Product"}</h2>
      {message && <Alert variant="success">{message}</Alert>}
      {error && <Alert variant="danger">{error}</Alert>}
      <Form onSubmit={handleSubmit}>
        <Form.Group className="mb-3">
          <Form.Label>Name</Form.Label>
          <Form.Control type="text" name="name" value={product.name} onChange={handleChange} required />
        </Form.Group>
        <Form.Group className="mb-3">
          <Form.Label>Description</Form.Label>
          <Form.Control as="textarea" name="description" value={product.description} onChange={handleChange} />
        </Form.Group>
        <Form.Group className="mb-3">
          <Form.Label>Price</Form.Label>
          <Form.Control type="number" name="price" value={product.price} onChange={handleChange} required />
        </Form.Group>
        <Form.Group className="mb-3">
          <Form.Label>Stock</Form.Label>
          <Form.Control type="number" name="stock" value={product.stock} onChange={handleChange} required />
        </Form.Group>
        <Form.Group className="mb-3">
          <Form.Label>Image</Form.Label>
          <Form.Control type="file" onChange={handleFileChange} />
        </Form.Group>
        <Button variant="primary" type="submit">{id ? "Update" : "Add"} Product</Button>
      </Form>
    </Container>
  );
};

export default ProductForm;

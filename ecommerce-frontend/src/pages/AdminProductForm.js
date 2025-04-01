import React, { useState, useEffect } from "react";
import { useSelector } from "react-redux";
import { getProducts, getProductById, addProduct, updateProduct, deleteProduct, setAuthToken } from "../Services/api";
import { Form, Button, Container, Alert, Card, Row, Col, Modal } from "react-bootstrap";
import defaultImage from "../assets/product/photo.png";

const ProductForm = () => {
  const token = useSelector((state) => state.auth.token);
  const [products, setProducts] = useState([]);
  const [product, setProduct] = useState({
    name: "",
    description: "",
    price: "",
    stock: "",
    imageUrl: "",
    image: null,
  });
  const [message, setMessage] = useState("");
  const [error, setError] = useState("");
  const [showModal, setShowModal] = useState(false);
  const [editProductId, setEditProductId] = useState(null);
  const API_URL = "https://localhost:7048";

  useEffect(() => {
    fetchProducts();
  }, []);

  const fetchProducts = () => {
    getProducts()
      .then((res) => setProducts(res.data))
      .catch(() => setError("Failed to load products"));
  };

  const handleShowModal = async (productId = null) => {
    setError("");
    setMessage("");
    setEditProductId(productId);

    if (productId) {
      try {
        const res = await getProductById(productId);
        setProduct(res.data);
      } catch {
        setError("Failed to load product details");
      }
    } else {
      setProduct({
        name: "",
        description: "",
        price: "",
        stock: "",
        imageUrl: "",
        image: null,
      });
    }

    setShowModal(true);
  };

  const handleCloseModal = () => {
    setShowModal(false);
    setEditProductId(null);
  };

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
    setAuthToken(token);

    try {
      const formData = new FormData();
      formData.append("name", product.name);
      formData.append("description", product.description);
      formData.append("price", product.price);
      formData.append("stock", product.stock);
      formData.append("imageUrl", "test");
      if (product.image) formData.append("image", product.image);

      if (editProductId) {
        await updateProduct(editProductId, formData);
        setMessage("Product updated successfully!");
      } else {
        await addProduct(formData);
        setMessage("Product added successfully!");
      }
      handleCloseModal();
      fetchProducts();
    } catch (err) {
      setError("Something went wrong. Please try again.");
    }
  };

  const handleDelete = async (productId) => {
    try {
      setAuthToken(token);
      await deleteProduct(productId);
      setMessage("Product deleted successfully!");
      fetchProducts();
    } catch (err) {
      setError("Failed to delete product.");
    }
  };
  const getProductImage = (imageUrl) => {
    return imageUrl ? `${API_URL}/${imageUrl}` : defaultImage;
  };

  return (
    <Container className="mt-4">
      <Button variant="info" onClick={() => handleShowModal()}>Add Product</Button>
      
      {message && <Alert variant="success" className="mt-3">{message}</Alert>}
      {error && <Alert variant="danger" className="mt-3">{error}</Alert>}

      <h2 className="mt-4">Product List</h2>
      <Row>
        {products.map((p) => (
          <Col md={4} key={p.productId} className="mb-3">
            <Card>
              <Card.Img variant="top"
                  src={getProductImage(p.imageUrl)}
                  style={{ height: "200px", objectFit: "cover" }}
                  alt={p.name} />
              <Card.Body>
                <Card.Title>{p.name}</Card.Title>
                <Card.Text>{p.description}</Card.Text>
                <Card.Text>Price: ${p.price}</Card.Text>
                <Button variant="warning" onClick={() => handleShowModal(p.productId)}>Edit</Button>
                <Button variant="danger" className="ms-2" onClick={() => handleDelete(p.productId)}>Delete</Button>
              </Card.Body>
            </Card>
          </Col>
        ))}
      </Row>

      {/* Modal for Add/Edit Product */}
      <Modal show={showModal} onHide={handleCloseModal} centered>
        <Modal.Header closeButton>
          <Modal.Title>{editProductId ? "Edit Product" : "Add Product"}</Modal.Title>
        </Modal.Header>
        <Modal.Body>
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
            <Button variant="primary" type="submit">{editProductId ? "Update" : "Add"} Product</Button>
          </Form>
        </Modal.Body>
      </Modal>
    </Container>
  );
};

export default ProductForm;

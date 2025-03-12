import React from "react";
import { useSelector, useDispatch } from "react-redux";
import { addToCart, decreaseQuantity, removeFromCart, clearCart } from "../redux/cartSlice";
import { Container, Row, Col, Button, Table } from "react-bootstrap";
import { toast } from "react-toastify";
import { useNavigate } from "react-router-dom";

import defaultImage from "../assets/product/photo.png";
const API_URL = "https://localhost:7048";

const Cart = () => {
  const navigate = useNavigate();
  const token = useSelector((state) => state.auth.token);

  const handleProceedToCheckout = () => {
    if (!token) {
      // Not logged in, redirect to login page
      navigate("/login");
    } else {
      // Logged in, redirect to checkout page
      navigate("/checkout");
    }
  };



  const cartItems = useSelector((state) => state.cart);
  const dispatch = useDispatch();

  const totalAmount = cartItems.reduce((total, item) => total + item.price * item.quantity, 0);

  const handleIncrease = (item) => {
    dispatch(addToCart(item));
    toast.info(`Increased quantity for ${item.name}`);
  };

  const handleDecrease = (item) => {
    if (item.quantity === 1) {
      toast.warn(`${item.name} removed from cart`);
    } else {
      toast.info(`Decreased quantity for ${item.name}`);
    }
    dispatch(decreaseQuantity(item.productId));
  };

  const handleRemove = (item) => {
    dispatch(removeFromCart(item.productId));
    toast.error(`${item.name} removed from cart`);
  };

  const handleClearCart = () => {
    dispatch(clearCart());
    toast.error("Cart cleared");
  };

  if (cartItems.length === 0) {
    return (
      <Container className="mt-4">
        <h2>Your Cart is Empty</h2>
      </Container>
    );
  }

  return (
    <Container className="mt-4">
      <h2>Your Cart</h2>
      <Table striped bordered hover responsive className="mt-3">
        <thead>
          <tr>
            <th>Image</th>
            <th>Product</th>
            <th>Price (Each)</th>
            <th>Quantity</th>
            <th>Total</th>
            <th>Action</th>
          </tr>
        </thead>
        <tbody>
          {cartItems.map((item) => (
            <tr key={item.productId}>
              <td style={{ width: "100px" }}>
                <img
                  src={item.imageUrl ? `${API_URL}/${item.imageUrl}` : defaultImage}
                  alt={item.name}
                  style={{ width: "100px", height: "80px", objectFit: "cover" }}
                />
              </td>
              <td>{item.name}</td>
              <td>${item.price.toFixed(2)}</td>
              <td>
                <Button
                  variant="outline-secondary"
                  size="sm"
                  onClick={() => handleDecrease(item)}
                  className="me-2"
                >
                  -
                </Button>
                {item.quantity}
                <Button
                  variant="outline-secondary"
                  size="sm"
                  onClick={() => handleIncrease(item)}
                  className="ms-2"
                >
                  +
                </Button>
              </td>
              <td>${(item.price * item.quantity).toFixed(2)}</td>
              <td>
                <Button
                  variant="danger"
                  size="sm"
                  onClick={() => handleRemove(item)}
                >
                  Remove
                </Button>
              </td>
            </tr>
          ))}
        </tbody>
      </Table>

      <Row className="mt-4">
        <Col>
          <h4>Total Amount: ${totalAmount.toFixed(2)}</h4>
        </Col>
        <Col className="text-end">
          <Button variant="danger" onClick={handleClearCart} className="me-2">
            Clear Cart
          </Button>
          <Button variant="success"  onClick={handleProceedToCheckout}
          >Proceed to Checkout</Button>
        </Col>
      </Row>
    </Container>
  );
};

export default Cart;

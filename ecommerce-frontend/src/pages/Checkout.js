import React, { useEffect, useState } from "react";
import { getCart, placeOrder } from "../Services/api";
import { useNavigate } from "react-router-dom";
import { Container, Table, Button, Alert } from "react-bootstrap";

const Checkout = () => {
  const [cartItems, setCartItems] = useState([]);
  const [message, setMessage] = useState("");
  const navigate = useNavigate();

  useEffect(() => {
    getCart().then((res) => setCartItems(res.data));
  }, []);

  const handlePlaceOrder = async () => {
    const res = await placeOrder({ items: cartItems });
    if (res.data.success) {
      setMessage("Order placed successfully!");
      setTimeout(() => navigate("/orders"), 2000);
    }
  };

  return (
    <Container className="mt-4">
      <h2>Checkout</h2>
      {message && <Alert variant="success">{message}</Alert>}
      <Table striped bordered>
        <thead>
          <tr>
            <th>Product</th>
            <th>Price</th>
            <th>Quantity</th>
          </tr>
        </thead>
        <tbody>
          {cartItems.map((item) => (
            <tr key={item.id}>
              <td>{item.name}</td>
              <td>${item.price}</td>
              <td>{item.quantity}</td>
            </tr>
          ))}
        </tbody>
      </Table>
      <Button onClick={handlePlaceOrder} variant="success">Place Order</Button>
    </Container>
  );
};

export default Checkout;

import React, { useEffect, useState } from "react";
import { placeOrder } from "../Services/api";
import { useNavigate } from "react-router-dom";
import { useSelector, useDispatch } from "react-redux";
import { Container, Table, Button, Alert } from "react-bootstrap";
import { clearCart } from "../redux/cartSlice";

const Checkout = () => {
  const [message, setMessage] = useState("");
  const [errorMessage, setErrorMessage] = useState("");

  const navigate = useNavigate();
  const token = useSelector((state) => state.auth.token);
  const cartItems = useSelector((state) => state.cart) || []; // Safe fallback
  const dispatch = useDispatch();

  useEffect(() => {
    if (!token) {
      navigate("/login");
    }
  }, [navigate, token]);

  const handlePlaceOrder = async () => {
    try {
      if (cartItems.length === 0) {
        setErrorMessage("Your cart is empty.");
        return;
      }
  
      // Extract only the required fields
      const orderItems = cartItems.map(({ productId, quantity, price }) => ({
        productId,
        quantity,
        price,
      }));
  
      const res = await placeOrder(orderItems); // Send only required data
  
      if (res.status === 200 || res.status === 201) {
        setMessage("Order placed successfully!");
        dispatch(clearCart());
        setTimeout(() => navigate("/orders"), 2000);
      } else {
        setErrorMessage("Failed to place order.");
      }
    } catch (error) {
      console.error("Error placing order:", error);
      setErrorMessage("An error occurred while placing the order.");
    }
  };
  

  return (
    <Container className="mt-4">
      <h2>Checkout</h2>
      {message && <Alert variant="success">{message}</Alert>}
      {errorMessage && <Alert variant="danger">{errorMessage}</Alert>}

      {cartItems.length === 0 ? (
        <Alert variant="warning">Your cart is empty!</Alert>
      ) : (
        <>
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
                <tr key={item.productId}>
                  <td>{item.name}</td>
                  <td>${item.price}</td>
                  <td>{item.quantity}</td>
                </tr>
              ))}
            </tbody>
          </Table>

          <Button onClick={handlePlaceOrder} variant="success">
            Place Order
          </Button>
        </>
      )}
    </Container>
  );
};

export default Checkout;

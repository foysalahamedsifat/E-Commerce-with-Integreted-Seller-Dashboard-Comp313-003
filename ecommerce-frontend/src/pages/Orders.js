import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { Container, Table, Alert } from "react-bootstrap";
import { useNavigate } from "react-router-dom";
import { getUserOrders ,setAuthToken} from "../Services/api";

const Orders = () => {
  const [orders, setOrders] = useState([]);
  const [errorMessage, setErrorMessage] = useState("");
  const token = useSelector((state) => state.auth.token);
  const user = useSelector((state) => state.auth.user); // Assuming you have user data in redux
  const navigate = useNavigate();

  useEffect(() => {
    if (!token) {
      navigate("/login");
      return;
    }
  
    const fetchOrders = async () => {
      try {
        // Optional: if you want to set the token globally
        setAuthToken(token);
  
        const res = await getUserOrders(token); // Pass token if not using global headers
  
        setOrders(res.data);
      } catch (error) {
        console.error("Error fetching orders:", error);
        setErrorMessage("Unable to fetch orders.");
      }
    };
  
    fetchOrders();
  }, [token, user.id, navigate]);

  return (
    <Container className="mt-4">
      <h2>My Orders</h2>
      {errorMessage && <Alert variant="danger">{errorMessage}</Alert>}

      {orders.length === 0 ? (
        <Alert variant="info">No orders found.</Alert>
      ) : (
        orders.map((order) => (
          <div key={order.orderId} className="mb-4 p-3 border rounded">
            <h4>Order #{order.orderId}</h4>
            <p>Date: {new Date(order.orderDate).toLocaleString()}</p>
            <p>Status: {order.status}</p>
            <p>Total Amount: ${order.totalAmount.toFixed(2)}</p>

            <Table striped bordered>
              <thead>
                <tr>
                  <th>Product</th>
                  <th>Price</th>
                  <th>Quantity</th>
                </tr>
              </thead>
              <tbody>
                {order.orderDetails.map((detail) => (
                  <tr key={detail.orderDetailId}>
                    <td>{detail.product.name}</td>
                    <td>${detail.price}</td>
                    <td>{detail.quantity}</td>
                  </tr>
                ))}
              </tbody>
            </Table>
          </div>
        ))
      )}
    </Container>
  );
};

export default Orders;

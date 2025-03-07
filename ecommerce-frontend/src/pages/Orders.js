import React, { useEffect, useState } from "react";
import { getOrders } from "../Services/api";
import { Container, Table } from "react-bootstrap";

const Orders = () => {
  const [orders, setOrders] = useState([]);

  useEffect(() => {
    getOrders().then((res) => setOrders(res.data));
  }, []);

  return (
    <Container className="mt-4">
      <h2>Order History</h2>
      <Table striped bordered>
        <thead>
          <tr>
            <th>Order ID</th>
            <th>Items</th>
            <th>Total Price</th>
            <th>Date</th>
          </tr>
        </thead>
        <tbody>
          {orders.map((order) => (
            <tr key={order.id}>
              <td>{order.id}</td>
              <td>
                {order.items.map((item) => (
                  <div key={item.id}>{item.name} x {item.quantity}</div>
                ))}
              </td>
              <td>${order.totalPrice}</td>
              <td>{new Date(order.date).toLocaleDateString()}</td>
            </tr>
          ))}
        </tbody>
      </Table>
    </Container>
  );
};

export default Orders;

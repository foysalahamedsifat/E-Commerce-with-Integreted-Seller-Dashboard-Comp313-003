import React, { useEffect, useState } from "react";
import { getAdminAnalytics } from "../Services/api";
import { Container, Card, Row, Col, Table } from "react-bootstrap";

const AdminDashboard = () => {
  const [analytics, setAnalytics] = useState({ totalSales: 0, totalRevenue: 0, bestSellers: [] });

  useEffect(() => {
    getAdminAnalytics().then((res) => setAnalytics(res.data));
  }, []);

  return (
    <Container className="mt-4">
      <h2>Admin Dashboard</h2>
      <Row>
        <Col md={4}>
          <Card className="mb-3">
            <Card.Body>
              <Card.Title>Total Sales</Card.Title>
              <Card.Text>{analytics.totalSales}</Card.Text>
            </Card.Body>
          </Card>
        </Col>
        <Col md={4}>
          <Card className="mb-3">
            <Card.Body>
              <Card.Title>Total Revenue</Card.Title>
              <Card.Text>${analytics.totalRevenue}</Card.Text>
            </Card.Body>
          </Card>
        </Col>
      </Row>
      <h4>Best-Selling Products</h4>
      <Table striped bordered>
        <thead>
          <tr>
            <th>Product</th>
            <th>Units Sold</th>
          </tr>
        </thead>
        <tbody>
          {analytics.bestSellers.map((product) => (
            <tr key={product.id}>
              <td>{product.name}</td>
              <td>{product.unitsSold}</td>
            </tr>
          ))}
        </tbody>
      </Table>
    </Container>
  );
};

export default AdminDashboard;

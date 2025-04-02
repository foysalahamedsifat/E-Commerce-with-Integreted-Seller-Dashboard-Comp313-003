import React, { useEffect, useState } from "react";
import { getDashboardSummary, getBestSellingProducts, getSalesReport, getUserStatistics, getOrderStatusDistribution, setAuthToken } from "../Services/api";
import { Bar, Pie, LineChart, Line, XAxis, YAxis, Tooltip, CartesianGrid, Legend, BarChart, PieChart, Cell } from "recharts";
import { format } from "date-fns";
import { useSelector } from "react-redux";
import { Spinner, Card, ListGroup, Container, Row, Col } from "react-bootstrap";

const AdminDashboard = () => {
  const [summary, setSummary] = useState(null);
  const [bestSelling, setBestSelling] = useState([]);
  const [salesReport, setSalesReport] = useState([]);
  const [userStats, setUserStats] = useState([]);
  const [orderStatus, setOrderStatus] = useState([]);

  const startDate = format(new Date().setDate(new Date().getDate() - 30), "yyyy-MM-dd");
  const endDate = format(new Date(), "yyyy-MM-dd");
  const token = useSelector((state) => state.auth.token);

  useEffect(() => {
    setAuthToken(token);
    getDashboardSummary().then((res) => setSummary(res.data));
    getBestSellingProducts().then((res) => setBestSelling(res.data));
    getSalesReport(startDate, endDate).then((res) => setSalesReport(res.data));
    getUserStatistics().then((res) => setUserStats(res.data));
    getOrderStatusDistribution().then((res) => setOrderStatus(res.data));
  }, [token]);

  if (!summary) {
    return (
      <div className="text-center mt-5">
        <Spinner animation="border" />
        <p>Loading Dashboard...</p>
      </div>
    );
  }

  const COLORS = ["#0088FE", "#00C49F", "#FFBB28", "#FF8042", "#8884d8"];

  // Function to safely format date
  const safeFormatDate = (date) => {
    const parsedDate = new Date(date);
    if (isNaN(parsedDate.getTime())) {
      return ''; // return empty string if date is invalid
    }
    return format(parsedDate, "MMM dd");
  };

  return (
    <Container className="mt-4">
      <h2 className="mb-4">Admin Dashboard</h2>
      <Row className="mb-4">
        <Col md={4}>
          <Card className="shadow-sm p-3 mb-4">
            <Card.Body>
              <Card.Title>Total Users</Card.Title>
              <Card.Text className="fs-4">{summary.totalUsers}</Card.Text>
            </Card.Body>
          </Card>
        </Col>
        <Col md={4}>
          <Card className="shadow-sm p-3 mb-4">
            <Card.Body>
              <Card.Title>Total Orders</Card.Title>
              <Card.Text className="fs-4">{summary.totalOrders}</Card.Text>
            </Card.Body>
          </Card>
        </Col>
        <Col md={4}>
          <Card className="shadow-sm p-3 mb-4">
            <Card.Body>
              <Card.Title>Total Revenue</Card.Title>
              <Card.Text className="fs-4">${summary.totalRevenue}</Card.Text>
            </Card.Body>
          </Card>
        </Col>
      </Row>

      <Row className="mb-4">
        <Col md={6}>
          <Card className="shadow-sm p-3 mb-4">
            <Card.Body>
              <Card.Title>Best Selling Products</Card.Title>
              <ListGroup variant="flush">
                {bestSelling.map((product) => (
                  <ListGroup.Item key={product.productId}>
                    {product.productName} - {product.totalQuantitySold} sold
                  </ListGroup.Item>
                ))}
              </ListGroup>
            </Card.Body>
          </Card>
        </Col>

        <Col md={6}>
          <Card className="shadow-sm p-3 mb-4">
            <Card.Body>
              <Card.Title>Order Status Distribution</Card.Title>
              <PieChart width={300} height={250}>
                <Pie
                  data={orderStatus}
                  dataKey="count"
                  nameKey="Status"
                  cx="50%"
                  cy="50%"
                  outerRadius={80}
                  fill="#8884d8"
                  label
                >
                  {orderStatus.map((entry, index) => (
                    <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                  ))}
                </Pie>
                <Tooltip />
              </PieChart>
            </Card.Body>
          </Card>
        </Col>
      </Row>

      <Row className="mb-4">
        <Col md={6}>
          <Card className="shadow-sm p-3 mb-4">
            <Card.Body>
              <Card.Title>User Statistics</Card.Title>
              <LineChart width={600} height={300} data={userStats}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="date" tickFormatter={safeFormatDate} />
                <YAxis />
                <Tooltip />
                <Legend />
                <Line type="monotone" dataKey="newUsers" stroke="#8884d8" />
              </LineChart>
            </Card.Body>
          </Card>
        </Col>

        <Col md={6}>
          <Card className="shadow-sm p-3 mb-4">
            <Card.Body>
              <Card.Title>Sales Report (Last 30 Days)</Card.Title>
              <BarChart width={600} height={300} data={salesReport}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="date" tickFormatter={safeFormatDate} />
                <YAxis />
                <Tooltip />
                <Legend />
                <Bar dataKey="totalRevenue" fill="#82ca9d" />
              </BarChart>
            </Card.Body>
          </Card>
        </Col>
      </Row>

      <Row>
        <Col md={12}>
          <Card className="shadow-sm p-3 mb-4">
            <Card.Body>
              <Card.Title>Sales Overview</Card.Title>
              <BarChart width={600} height={300} data={salesReport}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="date" tickFormatter={safeFormatDate} />
                <YAxis />
                <Tooltip />
                <Legend />
                <Bar dataKey="totalOrders" fill="#0088FE" />
                <Bar dataKey="totalRevenue" fill="#82ca9d" />
              </BarChart>
            </Card.Body>
          </Card>
        </Col>
      </Row>
    </Container>
  );
};

export default AdminDashboard;

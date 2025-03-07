import React from "react";
import { Link } from "react-router-dom";
import { useSelector, useDispatch } from "react-redux";
import { logout } from "../redux/authSlice";
import { Navbar, Nav, Container, Button } from "react-bootstrap";

const MyNavbar = () => {
  const { user } = useSelector((state) => state.auth);
  const dispatch = useDispatch();

  const handleLogout = () => {
    dispatch(logout());
  };

  return (
    <Navbar bg="dark" variant="dark" expand="lg">
      <Container>
        <Navbar.Brand as={Link} to="/">E-Commerce</Navbar.Brand>
        <Nav className="ml-auto">
          <Nav.Link as={Link} to="/cart">Cart</Nav.Link>
          {user ? (
            <>
              <Nav.Link as={Link} to="/orders">Orders</Nav.Link>
              <Button variant="outline-light" onClick={handleLogout}>Logout</Button>
            </>
          ) : (
            <Nav.Link as={Link} to="/login">Login</Nav.Link>
          )}
        </Nav>
      </Container>
    </Navbar>
  );
};

export default MyNavbar;

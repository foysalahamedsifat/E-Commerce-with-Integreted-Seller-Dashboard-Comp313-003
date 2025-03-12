import React from "react";
import { Link } from "react-router-dom";
import { useSelector, useDispatch } from "react-redux";
import { logout } from "../redux/authSlice";
import { Navbar, Nav, Container, Button, Badge } from "react-bootstrap";

const MyNavbar = () => {
  const user = useSelector((state) => state.auth.user);
  const roles = useSelector((state) => state.auth.roles);

  const cartItems = useSelector((state) => state.cart);
  const dispatch = useDispatch();

  const handleLogout = () => {
    dispatch(logout());
  };

  // Calculate total quantity in cart
  const cartCount = cartItems.reduce((total, item) => total + item.quantity, 0);

  // If roles is an array, check if it includes 'admin'
  const isAdmin = Array.isArray(roles) ? roles.includes("Admin") : roles === "Admin";

  return (
    <Navbar bg="dark" variant="dark" expand="lg">
      <Container>
        <Navbar.Brand as={Link} to="/">E-Commerce</Navbar.Brand>
        <Nav className="ml-auto">
          <Nav.Link as={Link} to="/cart">
            Cart{" "}
            {cartCount > 0 && (
              <Badge pill bg="danger">
                {cartCount}
              </Badge>
            )}
          </Nav.Link>

          {user ? (
            <>
              <Nav.Link as={Link} to="/orders">Orders</Nav.Link>
              <Button variant="outline-light" onClick={handleLogout}>
                {user} - Logout
              </Button>
            </>
          ) : (
            <>
              <Nav.Link as={Link} to="/login">Login</Nav.Link>
              <Nav.Link as={Link} to="/registration">Registration</Nav.Link>
            </>
          )}

          {/* This admin link will show only if isAdmin is true */}
          {isAdmin && (
            <Nav.Link as={Link} to="/registration-admin">Admin Register</Nav.Link>
          )}
        </Nav>
      </Container>
    </Navbar>
  );
};

export default MyNavbar;

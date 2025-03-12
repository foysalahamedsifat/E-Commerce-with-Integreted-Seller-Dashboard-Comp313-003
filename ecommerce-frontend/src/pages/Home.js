import React, { useEffect, useState } from "react";
import { getProducts } from "../Services/api";
import { Card, Button, Container, Row, Col } from "react-bootstrap";
import { useDispatch, useSelector } from "react-redux";
import { addToCart } from "../redux/cartSlice";
import defaultImage from "../assets/product/photo.png";
import { toast } from "react-toastify";

const API_URL = "https://localhost:7048";

const Home = () => {
  const [products, setProducts] = useState([]);
  const dispatch = useDispatch();
  const cartItems = useSelector((state) => state.cart);

  useEffect(() => {
    getProducts()
      .then((res) => {
        if (Array.isArray(res.data)) {
          setProducts(res.data);
        } else {
          console.error("Expected array, got:", res.data);
          setProducts([]);
        }
      })
      .catch((err) => {
        console.error("Failed to fetch products", err);
        setProducts([]);
      });
  }, []);

  const handleAddToCart = (product) => {
    const exists = cartItems.find((item) => item.id === product.productId);

    if (exists) {
      toast.info(`${product.name} quantity increased in cart`);
    } else {
      toast.success(`${product.name} added to cart`);
    }

    dispatch(addToCart(product));
  };

  const getProductImage = (imageUrl) => {
    return imageUrl ? `${API_URL}/${imageUrl}` : defaultImage;
  };

  return (
    <Container className="mt-4">
      <Row>
        {Array.isArray(products) && products.length > 0 ? (
          products.map((product) => (
            <Col md={4} key={product.productId}>
              <Card className="mb-4 shadow-sm">
                <Card.Img
                  variant="top"
                  src={getProductImage(product.imageUrl)}
                  style={{ height: "200px", objectFit: "cover" }}
                  alt={product.name}
                />
                <Card.Body>
                  <Card.Title>{product.name}</Card.Title>
                  <Card.Text>${product.price}</Card.Text>
                  <Button
                    variant="primary"
                    onClick={() => handleAddToCart(product)}
                  >
                    Add to Cart
                  </Button>
                </Card.Body>
              </Card>
            </Col>
          ))
        ) : (
          <p>No products available.</p>
        )}
      </Row>
    </Container>
  );
};

export default Home;
